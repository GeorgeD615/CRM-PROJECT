using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.DetachPosition;

public record DetachPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand;
