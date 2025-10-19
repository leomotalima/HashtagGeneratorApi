using HashtagGeneratorApi.Models;
using HashtagGeneratorApi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Serviços
// ----------------------
builder.Services.AddHttpClient<OllamaService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HashtagGenerator API",
        Version = "v1",
        Description = "API para gerar hashtags a partir de um texto."
    });
});

// Desserialização case-insensitive
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// ----------------------
// Middleware
// ----------------------
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

// ----------------------
// Endpoints
// ----------------------
app.MapPost("/hashtags", async ([FromBody] HashtagRequest req, OllamaService ollama) =>
{
    Console.WriteLine($"Texto recebido: {req.Text}, Count: {req.Count}, Modelo: {req.Model}");

    if (string.IsNullOrWhiteSpace(req.Text))
        return Results.BadRequest(new { error = "O campo 'text' é obrigatório e não pode estar vazio." });

    if (req.Count <= 0) req.Count = 10;
    else if (req.Count > 30) req.Count = 30;

    if (string.IsNullOrWhiteSpace(req.Model))
        req.Model = "llama3.2:3b";

    try
    {
        var resultado = await ollama.GerarHashtagsAsync(req.Text, req.Count, req.Model);

        if (resultado is null)
            return Results.Ok(new { hashtags = new[] { "#IA", "#AnaliseDeDados" } });

        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao chamar Ollama: " + ex.Message);
        return Results.Ok(new { hashtags = new[] { "#IA", "#AnaliseDeDados" } });
    }
})
.WithName("GerarHashtags")
.WithTags("Hashtags");

app.MapGet("/ping", () => Results.Ok(new { status = "API rodando 🚀" }))
   .WithName("Ping");

// ----------------------
// Exec

app.Run();