using Microsoft.OpenApi.Models;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Swagger
// ----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WSafe API Analysis",
        Version = "v1",
        Description = "API para análisis de riesgos y predicciones con OpenAI"
    });
});

// ----------------------
// CORS
// ----------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader());
});

// ----------------------
// OpenAI API Key (ENV)
// ----------------------
string? rawKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

// Saneamos: quitamos espacios/saltos de línea accidentales
var apiKey = rawKey?.Trim();

// Validaciones útiles para evitar 401 por formato
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new Exception("⚠ No se encontró la variable de entorno OPENAI_API_KEY. Configúrala antes de ejecutar.");
}
if (!apiKey.StartsWith("sk-"))
{
    throw new Exception("⚠ La OPENAI_API_KEY no inicia con 'sk-'. Revisa la clave copiada.");
}
if (apiKey.StartsWith("sk-sk-"))
{
    throw new Exception("⚠ La OPENAI_API_KEY tiene doble prefijo 'sk-'. Elimina el duplicado.");
}

// ----------------------
// OpenAI Client
// ----------------------
var openAiClient = new OpenAIClient(apiKey);
builder.Services.AddSingleton(openAiClient);

// ----------------------
// Controllers
// ----------------------
builder.Services.AddControllers();

var app = builder.Build();

// ----------------------
// Middleware
// ----------------------

// Habilitar Swagger SIEMPRE para facilitar pruebas (si prefieres solo en dev, cambia esta sección)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WSafe API Analysis v1");
});

app.UseHttpsRedirection();

app.UseCors(); // usa la política por defecto registrada arriba

app.MapControllers();

// Health-check simple (útil para verificar despliegue y CORS)
app.MapGet("/health", () => Results.Ok(new { status = "ok", utc = DateTime.UtcNow }));

app.Run();
