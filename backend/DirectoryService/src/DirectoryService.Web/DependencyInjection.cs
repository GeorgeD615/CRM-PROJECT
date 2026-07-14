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
        services.AddControllers();

        services.AddOpenApi();

        services.AddHealthChecks();

        services.AddInfrastructure(configuration);

        services.AddCore();

        return services;
    }
}
