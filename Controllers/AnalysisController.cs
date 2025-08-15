using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;

namespace WSafe.Api.Analysis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly OpenAIClient _openAiClient;

        public AnalysisController(OpenAIClient openAiClient)
        {
            _openAiClient = openAiClient;
        }

        [HttpPost("predict-incidents")]
        public async Task<IActionResult> PredictIncidents([FromBody] object input)
        {
            var chatClient = _openAiClient.GetChatClient("gpt-4o-mini");

            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateUserMessage($"Analiza y predice incidentes con los siguientes datos históricos: {input}")
            };

            var response = await chatClient.CompleteChatAsync(messages);
            return Ok(response.Value.Content[0].Text);
        }

        /// <summary>
        /// Audita documentos y sugiere mejoras según normativas.
        /// </summary>
        [HttpPost("audit-documents")]
        public async Task<IActionResult> AuditDocuments([FromBody] object documentText)
        {
            var chatClient = _openAiClient.GetChatClient("gpt-4o-mini");

            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage("Eres un auditor experto en seguridad laboral."),
                ChatMessage.CreateUserMessage($"Revisa el siguiente documento y sugiere áreas de mejora según normativas: {documentText}")
            };

            var response = await chatClient.CompleteChatAsync(messages);
            return Ok(response.Value.Content[0].Text);
        }

        /// <summary>
        /// Detecta patrones de actitudes inseguras en reportes de incidentes.
        /// </summary>
        [HttpPost("detect-unsafe-behaviors")]
        public async Task<IActionResult> DetectUnsafeBehaviors([FromBody] object incidentReports)
        {
            var chatClient = _openAiClient.GetChatClient("gpt-4o-mini");

            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage("Eres un analista experto en prevención de riesgos."),
                ChatMessage.CreateUserMessage($"Detecta patrones de actitudes inseguras en los siguientes reportes: {incidentReports}")
            };

            var response = await chatClient.CompleteChatAsync(messages);
            return Ok(response.Value.Content[0].Text);
        }
    }
}
