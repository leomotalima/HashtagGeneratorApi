
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using HashtagGeneratorApi.Models;

namespace HashtagGeneratorApi.Services;

public class OllamaService
{
    private readonly HttpClient _http;

    public OllamaService(HttpClient http)
    {
        _http = http;
    }

    // Agora aceita 3 parâmetros: text, count, model
    public async Task<HashtagResponse?> GerarHashtagsAsync(string text, int count, string model)
    {
        // JSON de exemplo para forçar saída estruturada
        var exemploJson = JsonSerializer.Serialize(new { hashtags = new[] { "#exemplo1", "#exemplo2" } });

        // Prompt estruturado e explícito
        var prompt = $"""
Gere exatamente {count} hashtags únicas, curtas, sem espaços e sem duplicadas
sobre o tema: "{text}".
Cada hashtag deve começar com '#'. 
Responda somente com um JSON válido conforme este exemplo: {exemploJson}
""";

        var body = new
        {
            model = model,
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

            var responseInterno = responseText.GetString();
            if (string.IsNullOrWhiteSpace(responseInterno))
            {
                Console.WriteLine("Resposta interna do Ollama está vazia");
                return null;
            }

            // 🧩 Extrai apenas o JSON válido (entre o primeiro '{' e o último '}')
            var inicio = responseInterno.IndexOf('{');
            var fim = responseInterno.LastIndexOf('}');
            if (inicio < 0 || fim <= inicio)
            {
                Console.WriteLine("Resposta do Ollama não contém JSON válido.");
                Console.WriteLine($"Conteúdo recebido: {responseInterno}");
                return null;
            }

            var jsonLimpo = responseInterno.Substring(inicio, (fim - inicio + 1));

            HashtagResponse? hashtags;
            try
            {
                hashtags = JsonSerializer.Deserialize<HashtagResponse>(jsonLimpo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao desserializar JSON limpo: {ex.Message}");
                Console.WriteLine($"JSON limpo recebido: {jsonLimpo}");
                return null;
            }

            if (hashtags == null || hashtags.Hashtags == null)
            {
                return null;
            }

            // Pós-processamento: limpa duplicatas, espaços e garante prefixo '#'
            hashtags.Model = model;
            hashtags.Hashtags = hashtags.Hashtags
                .Where(h => !string.IsNullOrWhiteSpace(h))
                .Select(h => h.Trim())
                .Select(h => h.StartsWith("#") ? h : "#" + h)
                .Select(h => h.Replace(" ", ""))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(count)
                .ToList();

            hashtags.Count = hashtags.Hashtags.Count;

            return hashtags;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao chamar Ollama: {ex.Message}");
            return null;
        }
    }
}
