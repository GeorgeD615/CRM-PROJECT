using DirectoryService.Core;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

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

        string connectionString = configuration.GetConnectionString("DirectoryServiceDb")
            ?? throw new InvalidOperationException("Connection string 'DirectoryServiceDb' is not configured.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history")));

        services.AddCore();

        return services;
    }
}
