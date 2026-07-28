using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.UpdateDepartment;

public record UpdateDepartmentCommand(Guid DepartmentId, UpdateDepartmentRequest UpdateDepartmentDto) : ICommand;
