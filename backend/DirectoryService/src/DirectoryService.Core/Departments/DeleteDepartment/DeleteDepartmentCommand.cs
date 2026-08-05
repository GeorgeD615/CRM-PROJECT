using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.DeleteDepartment;

public record DeleteDepartmentCommand(Guid DepartmentId) : ICommand;
