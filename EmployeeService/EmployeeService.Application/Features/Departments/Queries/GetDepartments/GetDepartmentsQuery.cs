using EmployeeService.Application.DTOs.Departments;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery : IRequest<IReadOnlyList<DepartmentResponseDto>>;
