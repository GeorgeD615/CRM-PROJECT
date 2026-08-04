using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest CreateDepartmentDto) : IValidatedCommand;
