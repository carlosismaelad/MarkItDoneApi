using MarkItDoneApi.Infra.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Adds native support to OpenAPI/Swagger
builder.Services.AddOpenApi();

// Adds controllers support
builder.Services.AddControllers();

// Adds services support
builder.Services.AddScoped<ConnectionFactory>();
builder.Services.AddScoped<DatabaseStatusChecker>();

var app = builder.Build();

// Configures pipeline requests
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Adds Scalar UI using the official package
    app.MapScalarApiReference();
    
    // Redirecting to easier access
    app.MapGet("/docs", () => Results.Redirect("/scalar/v1"));
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Simple root route
app.MapGet("/", () => "MarkItDone API OK!");

app.Run();
