using DirectoryService.Core.Database;
using Npgsql;
using System.Data;

namespace DirectoryService.Infrastructure.Postgres.Database;

/// <summary>
/// Реализация <see cref="IDbConnectionFactory"/> для PostgreSQL. Сама фабрика не держит состояния
/// и регистрируется singleton-ом; пул соединений остаётся за Npgsql.
/// </summary>
public sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection Create() => new NpgsqlConnection(connectionString);
}
