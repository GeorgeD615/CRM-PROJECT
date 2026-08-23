using DirectoryService.Core.Database;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres;

/// <summary>
/// Регистрация сервисов инфраструктурного слоя в DI-контейнере.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DirectoryServiceDb")
            ?? throw new InvalidOperationException("Connection string 'DirectoryServiceDb' is not configured.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history")));

        // Read-сторона получает тот же scoped-контекст, но только через контракт без операций записи.
        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<ILocationsRepository, EfCoreLocationsRepository>();
        services.AddScoped<IDepartmentsRepository, EfCoreDepartmentsRepository>();
        services.AddScoped<IPositionsRepository, EfCorePositionsRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}
