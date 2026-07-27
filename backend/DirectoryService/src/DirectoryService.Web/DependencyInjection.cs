using DirectoryService.Core;
using DirectoryService.Infrastructure.Postgres;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using System.Text.Json.Serialization;

namespace DirectoryService.Web;

/// <summary>
/// Регистрация всех зависимостей приложения в DI-контейнере.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWebDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // WriteAsJsonAsync (Envelope-путь и ExceptionMiddleware) использует Http JSON options.
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddOpenApi();

        services.AddHealthChecks();

        services.AddInfrastructure(configuration);

        services.AddCore();

        services.AddSerilogLogging(configuration);

        return services;
    }

    public static IServiceCollection AddSerilogLogging(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Sink-и, минимальный уровень и override-ы берём из конфигурации appsettings,
        // а обогащение задаём здесь — так гарантируем наличие нужных пакетов-энричеров
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(sp)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))
            .Enrich.WithProperty("ServiceName", "DirectoryService"));

        return services;
    }
}
