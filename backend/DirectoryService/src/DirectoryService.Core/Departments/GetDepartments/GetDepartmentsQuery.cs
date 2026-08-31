using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Departments.GetDepartments;

public record GetDepartmentsQuery(GetDepartmentsRequest GetDepartmentsDto) : IValidatedQuery;
