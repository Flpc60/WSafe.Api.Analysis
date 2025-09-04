using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ====== 1) SERVICES ======

// Controllers
builder.Services.AddControllers();

// CORS (misma política/orígenes para tu front)
const string CorsPolicy = "WSafeCors";
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p =>
{
    p.WithOrigins(
        "http://localhost:8080",
        "https://localhost:44300"
    // "https://tu-dominio"
    )
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
}));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WSafe.Api.Analysis",
        Version = "v1",
        Description = "Predict / Audit / Detect"
    });
});

var app = builder.Build();

// ====== 2) MIDDLEWARE PIPELINE ======

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WSafe.Api.Analysis v1");
});

// CORS
app.UseCors(CorsPolicy);

// HTTPS redirection (comenta para pruebas HTTP con curl si lo necesitas)
app.UseHttpsRedirection();

app.UseAuthorization();

// Preflight OPTIONS
app.MapMethods("{*path}", new[] { "OPTIONS" }, (HttpContext ctx) =>
{
    var origin = ctx.Request.Headers["Origin"].ToString();
    if (!string.IsNullOrEmpty(origin))
    {
        ctx.Response.Headers["Access-Control-Allow-Origin"] = origin;
        ctx.Response.Headers["Vary"] = "Origin";
    }
    ctx.Response.Headers["Access-Control-Allow-Credentials"] = "true";
    ctx.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
    ctx.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,PATCH,DELETE,OPTIONS";
    return Results.Ok();
});

app.MapControllers();

app.Run();
