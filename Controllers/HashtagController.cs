using Microsoft.AspNetCore.Mvc;
using HashtagGeneratorApi.Models;
using HashtagGeneratorApi.Services;
using System.Linq;

namespace HashtagGeneratorApi.Controllers
{
    [ApiController]
    [Route("hashtags")]
    public class HashtagController : ControllerBase
    {
        private readonly OllamaService _ollamaService;

        public HashtagController(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateHashtags([FromBody] HashtagRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "O corpo da requisição não pode ser nulo." });

            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "O campo 'text' é obrigatório e não pode estar vazio." });

            var model = string.IsNullOrWhiteSpace(request.Model) ? "llama3.2:3b" : request.Model;

            int count = request.Count > 0 ? request.Count : 10;
            if (count > 30) count = 30;

            try
            {
                // Chama o serviço
                var result = await _ollamaService.GerarHashtagsAsync(request.Text, count, model);

                // Pega apenas a lista de hashtags
                var hashtags = result.Hashtags ?? new List<string>();

                // Limpeza: começa com #, sem espaços, sem duplicatas
                hashtags = hashtags
                    .Where(h => !string.IsNullOrWhiteSpace(h))
                    .Select(h => h.Trim())
                    .Where(h => h.StartsWith("#"))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // Preencher até count, repetindo hashtags existentes se necessário
                while (hashtags.Count < count)
                {
                    if (hashtags.Count == 0)
                        hashtags.Add("#Hashtag"); // placeholder
                    else
                    {
                        hashtags.AddRange(hashtags);
                        hashtags = hashtags.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    }
                }

                // Garantir que não exceda o count
                hashtags = hashtags.Take(count).ToList();

                // CRIA NOVO OBJETO DE RESPONSE (não retorna o result original)
                var response = new HashtagResponse
                {
                    Model = model,
                    Count = hashtags.Count,
                    Hashtags = hashtags
                };

                return Ok(response); // ✅ retorna o novo objeto
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Erro ao gerar hashtags: {ex.Message}" });
            }
        }
    }
}
