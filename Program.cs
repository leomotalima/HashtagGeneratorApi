using HashtagGeneratorApi.Models;
using HashtagGeneratorApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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

// Configuração para JSON case-insensitive
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

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
    Console.WriteLine($"Texto recebido: {req.Texto}, Quantidade: {req.Quantidade}");

    if (string.IsNullOrWhiteSpace(req.Texto))
        return Results.BadRequest("Texto não pode ser vazio.");

    if (req.Quantidade <= 0)
        req.Quantidade = 1;

    var resultado = await ollama.GerarHashtagsAsync(req.Texto, req.Quantidade);

    return resultado is null
        ? Results.Problem("Erro ao gerar hashtags.")
        : Results.Ok(resultado);
})
.WithName("GerarHashtags");

app.MapGet("/ping", () => Results.Ok(new { status = "API rodando 🚀" }))
   .WithName("Ping");

// ----------------------
// Executa aplicação
// ----------------------
app.Run();
