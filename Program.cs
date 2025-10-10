using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 🔧 Lê configurações do Ollama via appsettings.json
var ollamaBaseUrl = builder.Configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
var ollamaModel = builder.Configuration["Ollama:Model"] ?? "llama3.2:3b";

// Modelo de resposta esperada
public record HashtagResponse([property: JsonPropertyName("hashtags")] List<string> Hashtags);
public record HashtagRequest(string Texto, int Quantidade = 5);

app.MapPost("/hashtags", async (HashtagRequest req) =>
{
    using var http = new HttpClient { BaseAddress = new Uri(ollamaBaseUrl) };

    var prompt = $"""
    Gere exatamente {req.Quantidade} hashtags curtas e relevantes sobre o seguinte texto:
    "{req.Texto}"

    Responda em JSON válido no formato:
    {{
        "hashtags": ["#tag1", "#tag2", ...]
    }}
    """;

    var payload = new
    {
        model = ollamaModel,
        prompt,
        stream = false,
        format = "json"
    };

    var response = await http.PostAsJsonAsync("/api/generate", payload);

    if (!response.IsSuccessStatusCode)
    {
        var erro = await response.Content.ReadAsStringAsync();
        return Results.Problem($"Erro ao chamar o Ollama: {erro}");
    }

    var result = await response.Content.ReadFromJsonAsync<JsonElement>();

    if (result.TryGetProperty("response", out var innerJson))
    {
        var hashtags = JsonSerializer.Deserialize<HashtagResponse>(innerJson.GetString() ?? "{}");
        return Results.Ok(hashtags);
    }

    return Results.BadRequest("Formato inesperado da resposta do Ollama.");
});

app.Run();
