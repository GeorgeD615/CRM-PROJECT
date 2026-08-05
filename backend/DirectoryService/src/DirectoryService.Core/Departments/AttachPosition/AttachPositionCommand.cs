using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.AttachPosition;

public record AttachPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand;
