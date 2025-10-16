using System.Net.Http.Json;
using System.Text.Json;
using HashtagGeneratorApi.Models;

namespace HashtagGeneratorApi.Services;

public class OllamaService
{
    private readonly HttpClient _http;

    public OllamaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<HashtagResponse?> GerarHashtagsAsync(string texto, int quantidade)
    {
        // Exemplo de JSON de referência para o prompt
        var exemploJson = JsonSerializer.Serialize(new { hashtags = new[] { "#exemplo1", "#exemplo2" } });

        // Prompt usando interpolação segura
        var prompt = $"""
Gere exatamente {quantidade} hashtags curtas e relevantes sobre o seguinte texto:
"{texto}"
Responda em formato JSON válido no esquema: {exemploJson}
""";

        var body = new
        {
            model = "llama3",
            prompt,
            stream = false,
            format = "json"
        };

        var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", body);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (result.TryGetProperty("response", out var responseText))
        {
            try
            {
                var json = JsonSerializer.Deserialize<HashtagResponse>(responseText.GetString() ?? "{}");
                return json;
            }
            catch
            {
                return null;
            }
        }

        return null;
    }
}
