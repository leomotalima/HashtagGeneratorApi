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
                                .Select(h => h.StartsWith("#") ? h : "#" + h)
                                .Select(h => h.Replace(" ", ""))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .Take(count)
                                .ToList();

            if (!hashtags.Any())
                return Fallback(count, model);

            return new HashtagResponse
            {
                Model = model,
                Count = hashtags.Count,
                Hashtags = hashtags
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao chamar Ollama: {ex.Message}");
            return Fallback(count, model);
        }
    }

    private HashtagResponse Fallback(int count, string model) =>
        new HashtagResponse
        {
            Model = model,
            Count = Math.Min(count, 2),
            Hashtags = new List<string> { "#IA", "#AnaliseDeDados" }
        };
}
