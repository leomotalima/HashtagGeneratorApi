using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Modelo de resposta esperada do Ollama
public record HashtagResponse([property: JsonPropertyName("hashtags")] List<string> Hashtags);

// Modelo da requisição local
public record HashtagRequest(string Texto, int Quantidade = 5);

// Endpoint principal
app.MapPost("/hashtags", async (HashtagRequest req) =>
{
    using var http = new HttpClient { BaseAddress = new Uri("http://localhost:11434") };

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
        model = "llama3.2:3b",
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
