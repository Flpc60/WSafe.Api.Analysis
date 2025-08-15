using Microsoft.OpenApi.Models;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Habilitar Swagger con información básica
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

// Cargar clave desde variable de entorno
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    throw new Exception("⚠ No se encontró la variable de entorno OPENAI_API_KEY. Configúrala antes de ejecutar.");
}

// Registrar cliente OpenAI
var openAiClient = new OpenAIClient(apiKey);
builder.Services.AddSingleton(openAiClient);

// Controladores
builder.Services.AddControllers();

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS (para consumir desde tu app MVC externa)
app.UseCors(p => p
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
