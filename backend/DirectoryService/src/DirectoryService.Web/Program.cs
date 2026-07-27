using DirectoryService.Web;
using DirectoryService.Web.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using System.Globalization;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddWebDependencies(builder.Configuration);

    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = builder.Environment.IsDevelopment();
        options.ValidateOnBuild = builder.Environment.IsDevelopment();
    });

    WebApplication app = builder.Build();

    app.UseSerilogRequestLogging(options =>
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            diagnosticContext.Set("RequestId", httpContext.TraceIdentifier));

    app.UseExceptionMiddleware();

    app.MapHealthChecks("/api/health");

    app.MapControllers();

    if (!app.Environment.IsProduction())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}