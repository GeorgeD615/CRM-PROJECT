using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.AttachLocation;

public record AttachLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;
