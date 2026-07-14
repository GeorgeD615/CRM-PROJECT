using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Database;

/// <summary>
/// Фабрика соединений с PostgreSQL для репозиториев, работающих через Dapper.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Создаёт и открывает новое соединение с базой данных.
    /// </summary>
    Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}
