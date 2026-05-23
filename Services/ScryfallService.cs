using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace MTGRoyal.Services
{
    public class ScryfallService
    {
        private const int MaxCards = 5;
        private readonly HttpClient httpClient;
        private readonly IMemoryCache cache;

        // Recibe el cliente HTTP y la cache usada para consultas a Scryfall.
        public ScryfallService(HttpClient httpClient, IMemoryCache cache)
        {
            this.httpClient = httpClient;
            this.cache = cache;
        }

        // Construye un resumen corto de cartas relacionadas con el prompt.
        public async Task<string> BuildCompactContextAsync(string prompt)
        {
            var cardNames = ExtractCardNames(prompt);
            var cards = new List<ScryfallCard>();

            foreach (var cardName in cardNames.Take(3))
            {
                var card = await GetNamedCardAsync(cardName);

                if (card != null)
                    cards.Add(card);
            }

            if (cards.Count == 0)
            {
                cards.AddRange(await SearchCardsAsync(prompt));
            }

            if (cards.Count == 0)
                return string.Empty;

            return BuildSummary(cards.Take(MaxCards));
        }

        // Busca una carta por nombre aproximado en Scryfall.
        private async Task<ScryfallCard?> GetNamedCardAsync(string name)
        {
            var cacheKey = $"scryfall:named:{name.Trim().ToLowerInvariant()}";

            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);

                var url = $"cards/named?fuzzy={Uri.EscapeDataString(name)}";

                try
                {
                    return await httpClient.GetFromJsonAsync<ScryfallCard>(url);
                }
                catch
                {
                    return null;
                }
            });
        }

        // Busca cartas usando una consulta generada desde el prompt.
        private async Task<IReadOnlyList<ScryfallCard>> SearchCardsAsync(string prompt)
        {
            var query = BuildSearchQuery(prompt);

            if (string.IsNullOrWhiteSpace(query))
                return [];

            var cacheKey = $"scryfall:search:{query.ToLowerInvariant()}";

            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                var url = $"cards/search?q={Uri.EscapeDataString(query)}&unique=cards&order=edhrec";

                try
                {
                    var result = await httpClient.GetFromJsonAsync<ScryfallSearchResult>(url);
                    return result?.Data?.Take(MaxCards).ToList() ?? [];
                }
                catch
                {
                    return [];
                }
            }) ?? [];
        }

        // Extrae nombres de cartas escritos entre corchetes o comillas.
        private static List<string> ExtractCardNames(string prompt)
        {
            var names = new List<string>();

            foreach (Match match in Regex.Matches(prompt, @"\[\[(.+?)\]\]"))
            {
                names.Add(match.Groups[1].Value.Trim());
            }

            foreach (Match match in Regex.Matches(prompt, "\"(.+?)\""))
            {
                names.Add(match.Groups[1].Value.Trim());
            }

            return names
                .Where(name => name.Length > 1)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Convierte palabras clave del prompt en filtros de busqueda.
        private static string BuildSearchQuery(string prompt)
        {
            var normalized = prompt.ToLowerInvariant();
            var terms = new List<string>();

            if (normalized.Contains("commander") || normalized.Contains("edh"))
                terms.Add("legal:commander");

            if (normalized.Contains("modern"))
                terms.Add("legal:modern");

            if (normalized.Contains("standard"))
                terms.Add("legal:standard");

            if (normalized.Contains("pioneer"))
                terms.Add("legal:pioneer");

            if (normalized.Contains("pauper"))
                terms.Add("legal:pauper");

            if (normalized.Contains("criatura"))
                terms.Add("t:creature");

            if (normalized.Contains("artefacto"))
                terms.Add("t:artifact");

            if (normalized.Contains("encantamiento"))
                terms.Add("t:enchantment");

            if (normalized.Contains("instantaneo") || normalized.Contains("instantáneo"))
                terms.Add("t:instant");

            if (normalized.Contains("conjuro"))
                terms.Add("t:sorcery");

            if (normalized.Contains("planeswalker"))
                terms.Add("t:planeswalker");

            if (normalized.Contains("tierra"))
                terms.Add("t:land");

            if (terms.Count == 0)
                return string.Empty;

            return string.Join(" ", terms);
        }

        // Resume los datos importantes de las cartas encontradas.
        private static string BuildSummary(IEnumerable<ScryfallCard> cards)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Datos compactos de Scryfall para esta consulta:");

            foreach (var card in cards)
            {
                builder.Append("- ");
                builder.Append(card.Name);

                if (!string.IsNullOrWhiteSpace(card.ManaCost))
                    builder.Append($" | Coste: {card.ManaCost}");

                if (!string.IsNullOrWhiteSpace(card.TypeLine))
                    builder.Append($" | Tipo: {card.TypeLine}");

                if (!string.IsNullOrWhiteSpace(card.OracleText))
                    builder.Append($" | Texto: {Truncate(card.OracleText.ReplaceLineEndings(" "), 360)}");

                if (!string.IsNullOrWhiteSpace(card.SetName))
                    builder.Append($" | Coleccion: {card.SetName}");

                if (!string.IsNullOrWhiteSpace(card.Rarity))
                    builder.Append($" | Rareza: {card.Rarity}");

                if (!string.IsNullOrWhiteSpace(card.Prices?.Usd))
                    builder.Append($" | Precio USD aprox: {card.Prices.Usd}");

                builder.AppendLine();
            }

            return builder.ToString();
        }

        // Recorta textos largos para reducir tokens enviados a la IA.
        private static string Truncate(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            return value[..maxLength] + "...";
        }

        private sealed class ScryfallSearchResult
        {
            [JsonPropertyName("data")]
            public List<ScryfallCard>? Data { get; set; }
        }

        private sealed class ScryfallCard
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("mana_cost")]
            public string? ManaCost { get; set; }

            [JsonPropertyName("type_line")]
            public string? TypeLine { get; set; }

            [JsonPropertyName("oracle_text")]
            public string? OracleText { get; set; }

            [JsonPropertyName("set_name")]
            public string? SetName { get; set; }

            [JsonPropertyName("rarity")]
            public string? Rarity { get; set; }

            [JsonPropertyName("prices")]
            public ScryfallPrices? Prices { get; set; }
        }

        private sealed class ScryfallPrices
        {
            [JsonPropertyName("usd")]
            public string? Usd { get; set; }
        }
    }
}
