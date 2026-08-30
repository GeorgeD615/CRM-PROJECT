using System.Data;

namespace DirectoryService.Core.Database;

/// <summary>
/// Фабрика подключений к БД для read-сценариев на Dapper. Отдаёт новое подключение
/// на каждый запрос — владение и освобождение остаются за вызывающим кодом.
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection Create();
}
