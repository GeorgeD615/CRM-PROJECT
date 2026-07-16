namespace DirectoryService.Core.Database;

/// <summary>
/// Фиксирует накопленные изменения как одну атомарную операцию.
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// Атомарно сохраняет все накопленные изменения.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
