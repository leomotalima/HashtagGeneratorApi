using HashtagGeneratorApi.Models;
using HashtagGeneratorApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Configurações de serviços
// ----------------------
builder.Services.AddHttpClient<OllamaService>(); // Injeta HttpClient no OllamaService
builder.Services.AddEndpointsApiExplorer();       // Necessário para Swagger
builder.Services.AddSwaggerGen(c =>              // Configuração Swagger
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HashtagGenerator API",
        Version = "v1",
        Description = "API para gerar hashtags a partir de um texto."
    });
});

// (Opcional) CORS, útil se tiver frontend consumindo a API
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

app.UseCors(); // Ativa CORS

// ----------------------
// Endpoints
// ----------------------
app.MapPost("/hashtags", async (HashtagRequest req, OllamaService ollama) =>
{
    if (string.IsNullOrWhiteSpace(req.Texto))
        return Results.BadRequest("Texto não pode ser vazio.");

    if (req.Quantidade <= 0)
        req.Quantidade = 1;

    var resultado = await ollama.GerarHashtagsAsync(req.Texto, req.Quantidade);

    return resultado is null
        ? Results.Problem("Erro ao gerar hashtags.")
        : Results.Ok(resultado);
})
.WithName("GerarHashtags"); // Removed WithOpenApi()

app.MapGet("/ping", () => Results.Ok(new { status = "API rodando 🚀" }))
   .WithName("Ping"); // Removed WithOpenApi()

// ----------------------
// Executa aplicação
// ----------------------
app.Run();
