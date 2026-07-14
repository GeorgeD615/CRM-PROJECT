using DirectoryService.Web;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebDependencies(builder.Configuration);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = builder.Environment.IsDevelopment();
    options.ValidateOnBuild = builder.Environment.IsDevelopment();
});

WebApplication app = builder.Build();

app.MapHealthChecks("/api/health");

app.MapControllers();

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

await app.RunAsync();
