using Microsoft.Extensions.Options;
using MTGRoyal.Models;
using OpenAI;
using OpenAI.Chat;

namespace MTGRoyal.Services
{
    public class IAService
    {
        private readonly ChatClient chat;
        private readonly ScryfallService scryfallService;

        public IAService(
            IOptions<ConfiguracionIA> config,
            ScryfallService scryfallService)
        {
            var info = config.Value;
            var cliente = new OpenAIClient(info.ApiKey);

            chat = cliente.GetChatClient(info.ModeloTexto);
            this.scryfallService = scryfallService;
        }

        public async Task<string> GenerarTexto(string prompt)
        {
            var contextoScryfall =
                await scryfallService.BuildCompactContextAsync(prompt);

            var contexto =
                string.IsNullOrWhiteSpace(contextoScryfall)
                    ? "No se encontro contexto especifico en Scryfall para esta consulta."
                    : contextoScryfall;

            var resultado = await chat.CompleteChatAsync(
                new ChatMessage[]
                {
                    ChatMessage.CreateSystemMessage(
                        "Eres un experto en Magic: The Gathering. Ayuda a crear mazos, explicar cartas, reglas, estrategias y recomendaciones. Usa el contexto de Scryfall cuando este disponible. Si no hay datos suficientes, dilo claramente y pide el nombre exacto de la carta o formato. Responde de forma breve y util para ahorrar tokens."),

                    ChatMessage.CreateUserMessage(
                        $"Contexto externo:\n{contexto}\n\nPregunta del usuario:\n{prompt}")
                });

            return resultado.Value.Content[0].Text;
        }
    }
}
