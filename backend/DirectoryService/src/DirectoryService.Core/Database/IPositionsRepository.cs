using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища должностей: добавляет, загружает и удаляет сущности в текущем контексте,
/// но не фиксирует изменения — commit остаётся за <see cref="ITransactionManager"/>.
/// </summary>
public interface IPositionsRepository
{
    /// <summary>
    /// Возвращает должность по id; <see cref="ErrorType.NotFound"/>, если она не найдена.
    /// </summary>
    Task<Result<Position, Failure>> GetByIdAsync(PositionId id, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет, занято ли имя должности.
    /// </summary>
    Task<bool> IsNameTakenAsync(PositionName name, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет, привязана ли должность хотя бы к одному подразделению.
    /// </summary>
    Task<bool> HasLinkedDepartmentsAsync(PositionId id, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет новую должность в текущий контекст (без сохранения).
    /// </summary>
    void Add(Position position);

    /// <summary>
    /// Удаляет должность из текущего контекста (без сохранения).
    /// </summary>
    void Remove(Position position);
}
