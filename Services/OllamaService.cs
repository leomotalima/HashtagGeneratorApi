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
        // Ajuste count padrão e máximo
        if (count <= 0) count = 10;
        if (count > 30) count = 30;

        var prompt = $"""
Liste exatamente {count} hashtags únicas relacionadas ao tema: "{text}".
Cada hashtag deve começar com o símbolo '#' e estar em uma nova linha.
Não inclua explicações, apenas a lista direta.
""";

        var body = new { model, prompt, stream = false };

        try
        {
            Console.WriteLine("=== Enviando para Ollama ===");
            Console.WriteLine(prompt);

            var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", body);
            Console.WriteLine($"Status da resposta: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Erro na chamada Ollama: {response.StatusCode}");

            var resultText = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Resposta bruta do Ollama:");
            Console.WriteLine(resultText);

            // Extrai o campo "response" do JSON
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

            // Preenche se vier menos do que o solicitado
            if (hashtags.Count < count)
            {
                var extras = new List<string> {
                    "#Tech", "#Inovacao", "#Negocios", "#Desenvolvimento",
                    "#Startups", "#AI", "#DataScience", "#Automacao",
                    "#Marketing", "#Inovação", "#Empreendedorismo", "#Futuro",
                    "#Tecnologia", "#RedesSociais", "#Digital", "#Analytics"
                };

                foreach (var extra in extras)
                {
                    if (hashtags.Count >= count) break;
                    if (!hashtags.Contains(extra, StringComparer.OrdinalIgnoreCase))
                        hashtags.Add(extra);
                }

                // Caso ainda não tenha chegado ao count, adiciona placeholders
                while (hashtags.Count < count)
                {
                    hashtags.Add("#Hashtag" + (hashtags.Count + 1));
                }
            }

            return new HashtagResponse
            {
                Model = model,
                Count = count,
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
        var defaultHashtags = new List<string>
        {
            "#IA", "#AnaliseDeDados", "#Tech", "#MachineLearning"
        };

        // Completa até count
        while (defaultHashtags.Count < count)
        {
            defaultHashtags.Add("#Hashtag" + (defaultHashtags.Count + 1));
        }

        return new HashtagResponse
        {
            Model = model,
            Count = count,
            Hashtags = defaultHashtags.Take(count).ToList()
        };
    }
}
