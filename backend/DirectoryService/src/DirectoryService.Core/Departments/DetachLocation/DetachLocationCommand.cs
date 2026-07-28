using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.DetachLocation;

public record DetachLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;
