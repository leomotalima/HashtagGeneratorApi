using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using HashtagGeneratorApi.Models;

namespace HashtagGeneratorApi.Services;

public class OllamaService
{
    private readonly HttpClient _http;

    public OllamaService(HttpClient http) => _http = http;

    public async Task<HashtagResponse> GerarHashtagsAsync(string text, int count, string model)
    {
        var prompt = $"""
Crie exatamente {count} hashtags únicas sobre: "{text}".
Cada hashtag deve começar com '#'. Responda apenas com texto, sem explicações.
""";

        var body = new { model, prompt, stream = false };

        try
        {
            Console.WriteLine("=== Enviando para Ollama ===");
            Console.WriteLine(prompt);

            var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", body);
            Console.WriteLine($"Status da resposta: {response.StatusCode}");

            var resultText = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Resposta bruta do Ollama:");
            Console.WriteLine(resultText);

            // Tenta extrair o campo "response"
            string texto;
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(resultText);
                texto = json.GetProperty("response").GetString() ?? "";
            }
            catch
            {
                texto = resultText; // fallback para texto puro
            }

            if (string.IsNullOrWhiteSpace(texto))
                return Fallback(count, model);

            // Extrai hashtags
            var hashtags = Regex.Matches(texto, @"#\w+")
                                .Select(m => m.Value.Trim())
                                .Select(h => h.Replace(" ", ""))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .Take(count)
                                .ToList();

            // Se vier menos do que o solicitado, preenche com extras
            if (hashtags.Count < count)
            {
                var extras = new List<string> { "#Tech", "#Inovacao", "#Negocios", "#Desenvolvimento", "#Startups", "#AI", "#DataScience", "#Automacao" };

                foreach (var extra in extras)
                {
                    if (hashtags.Count >= count) break;
                    if (!hashtags.Contains(extra, StringComparer.OrdinalIgnoreCase))
                        hashtags.Add(extra);
                }
            }

            return new HashtagResponse
            {
                Model = model,
                Count = count, // sempre retorna o que foi solicitado
                Hashtags = hashtags.Take(count).ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao chamar Ollama: {ex.Message}");
            return Fallback(count, model);
        }
    }

    private HashtagResponse Fallback(int count, string model)
    {
        var defaultHashtags = new List<string> { "#IA", "#AnaliseDeDados", "#Tech", "#MachineLearning" };

        return new HashtagResponse
        {
            Model = model,
            Count = Math.Min(count, defaultHashtags.Count),
            Hashtags = defaultHashtags.Take(count).ToList()
        };
    }
}
