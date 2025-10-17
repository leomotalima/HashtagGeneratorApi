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

        // Prompt seguro
        var prompt = $"""
Gere exatamente {quantidade} hashtags curtas e relevantes sobre o seguinte texto:
"{texto}"
Responda em formato JSON válido no esquema: {exemploJson}
""";

        var body = new
        {
            model = "llama3", // modelo leve instalado
            prompt,
            stream = false,
            format = "json"
        };

        try
        {
            var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", body);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Erro Ollama: {response.StatusCode}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!result.TryGetProperty("response", out var responseText))
            {
                Console.WriteLine("O JSON retornado pelo Ollama não contém 'response'");
                return null;
            }

            // Desserializa o JSON interno que está como string dentro de "response"
            var responseInterno = responseText.GetString();
            if (string.IsNullOrWhiteSpace(responseInterno))
            {
                Console.WriteLine("Resposta interna do Ollama está vazia");
                return null;
            }

            try
            {
                var hashtags = JsonSerializer.Deserialize<HashtagResponse>(responseInterno);
                return hashtags;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao desserializar JSON interno: {ex.Message}");
                Console.WriteLine($"Conteúdo retornado: {responseInterno}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao chamar Ollama: {ex.Message}");
            return null;
        }
    }
}
