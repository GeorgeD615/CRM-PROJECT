using DirectoryService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

string connectionString = builder.Configuration.GetConnectionString("DirectoryServiceDb")
    ?? throw new InvalidOperationException("Connection string 'DirectoryServiceDb' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

WebApplication app = builder.Build();

app.MapHealthChecks("/api/health");

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

await app.RunAsync();
