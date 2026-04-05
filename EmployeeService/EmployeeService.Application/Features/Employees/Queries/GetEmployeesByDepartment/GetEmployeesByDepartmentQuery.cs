using EmployeeService.Application.DTOs.Employees;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeesByDepartment;

public record GetEmployeesByDepartmentQuery(int DepartmentId) : IRequest<IReadOnlyList<EmployeeListItemDto>>;
