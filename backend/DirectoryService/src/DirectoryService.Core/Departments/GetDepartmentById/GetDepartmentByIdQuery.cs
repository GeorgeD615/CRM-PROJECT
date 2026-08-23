using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.GetDepartmentById;

public record GetDepartmentByIdQuery(Guid DepartmentId) : IQuery;
