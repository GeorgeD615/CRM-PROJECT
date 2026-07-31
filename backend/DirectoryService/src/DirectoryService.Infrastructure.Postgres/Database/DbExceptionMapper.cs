using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Database;

/// <summary>
/// Единственное место, где технические исключения EF Core / Postgres превращаются в типизированные
/// доменные <see cref="Error"/>. Наружу не уходят SQL-текст, stack trace и имена constraint-ов —
/// только безопасные категории: concurrency / unique / foreign key / общий сбой БД.
/// </summary>
internal static class DbExceptionMapper
{
    public static Failure Map(Exception exception) => exception switch
    {
        DbUpdateConcurrencyException => Concurrency(),

        DbUpdateException { InnerException: PostgresException postgres } => MapPostgres(postgres),

        PostgresException postgres => MapPostgres(postgres),

        _ => GeneralFailure(),
    };

    /// <summary>
    /// Технический сбой (общая ошибка БД) логируется как Error; ожидаемые конфликты — как Warning.
    /// </summary>
    public static bool IsTechnical(Failure failure) => failure.Any(error => error.Type == ErrorType.Internal);

    private static Failure MapPostgres(PostgresException postgres) => postgres.SqlState switch
    {
        PostgresErrorCodes.UniqueViolation => Error.Conflict(
            "Запись с такими данными уже существует.",
            invalidProperty: ResolveField(postgres.ConstraintName),
            code: "database.unique_violation"),

        PostgresErrorCodes.ForeignKeyViolation => Error.Conflict(
            "Операция нарушает ссылочную целостность данных.",
            code: "database.foreign_key_violation"),

        PostgresErrorCodes.SerializationFailure or PostgresErrorCodes.DeadlockDetected => Concurrency(),

        _ => GeneralFailure(),
    };

    private static Failure Concurrency() => Error.Conflict(
        "Данные были изменены другим запросом. Повторите операцию.",
        code: "database.concurrency_conflict");

    private static Failure GeneralFailure() => Error.Internal(
        "Произошла ошибка при работе с базой данных.",
        code: "database.failure");

    // Constraint name используется только для выбора понятного поля; наружу он не уходит.
    private static string? ResolveField(string? constraintName) => constraintName switch
    {
        null => null,
        var name when name.Contains("name", StringComparison.OrdinalIgnoreCase) => "Name",
        var name when name.Contains("path", StringComparison.OrdinalIgnoreCase) => "Path",
        var name when name.Contains("slug", StringComparison.OrdinalIgnoreCase) => "Slug",
        _ => null,
    };
}
