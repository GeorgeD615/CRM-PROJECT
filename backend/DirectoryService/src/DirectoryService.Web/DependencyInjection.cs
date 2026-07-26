using System.Text.Json.Serialization;
using DirectoryService.Core;
using DirectoryService.Infrastructure.Postgres;

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
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // WriteAsJsonAsync (Envelope-путь и ExceptionMiddleware) использует Http JSON options.
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddOpenApi();

        services.AddHealthChecks();

        services.AddInfrastructure(configuration);

        services.AddCore();

        return services;
    }
}
