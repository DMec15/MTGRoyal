using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using MTGRoyal.Models;

namespace MTGRoyal.Services
{
    public class IAService
    {
        private readonly ChatClient chat;
        
        public IAService(IOptions<ConfiguracionIA> config)
        {
            var info = config.Value;
            var cliente = new OpenAIClient(info.ApiKey);

            chat = cliente.GetChatClient(info.ModeloTexto);
        }

        public async Task<string> GenerarTexto(string prompt)
        {
            var resultado = await chat.CompleteChatAsync(
                new ChatMessage[]
                {
                    ChatMessage.CreateSystemMessage("Eres un experto en Magic: The Gathering y tu tarea es ayudar a los usuarios a crear mazos de cartas. Responde a las preguntas sobre estrategias, combinaciones de cartas y recomendaciones para construir mazos efectivos. También debes proporcionar información vigente sobre precios y disponibilidad de cartas, así como sugerencias para mejorar los mazos existentes. Asegúrate de ser claro, conciso y útil en tus respuestas."),
                    ChatMessage.CreateUserMessage(prompt)
                }
            );

            return resultado.Value.Content[0].Text;
        }

    }
}