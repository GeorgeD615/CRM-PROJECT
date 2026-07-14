using System.Text.Json;
using Dapper;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Infrastructure.Postgres.Database;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище локаций на Dapper: явный SQL через соединение из <see cref="IDbConnectionFactory"/>.
/// </summary>
public sealed class DapperLocationsRepository : ILocationsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DapperLocationsRepository> _logger;

    public DapperLocationsRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<DapperLocationsRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<bool> IsNameTakenAsync(LocationName name, CancellationToken cancellationToken)
    {
        const string sql = "SELECT EXISTS (SELECT 1 FROM locations WHERE name = @Name);";

        await using NpgsqlConnection connection =
            await _connectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Name = name.Value }, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO locations (id, name, address, created_at, updated_at)
            VALUES (@Id, @Name, CAST(@Address AS jsonb), @CreatedAt, @UpdatedAt);
            """;

        // Имена json-свойств повторяют настройку ToJson в LocationConfiguration,
        // чтобы обе реализации хранили адрес в одном формате.
        string addressJson = JsonSerializer.Serialize(new
        {
            city = location.Address.City,
            street = location.Address.Street,
            house = location.Address.House,
            apartment = location.Address.Apartment,
        });

        try
        {
            await using NpgsqlConnection connection =
                await _connectionFactory.CreateConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(new CommandDefinition(
                sql,
                new
                {
                    Id = location.Id.Value,
                    Name = location.Name.Value,
                    Address = addressJson,
                    location.CreatedAt,
                    location.UpdatedAt,
                },
                cancellationToken: cancellationToken));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to save location {LocationId}.", location.Id.Value);

            throw;
        }
    }
}
