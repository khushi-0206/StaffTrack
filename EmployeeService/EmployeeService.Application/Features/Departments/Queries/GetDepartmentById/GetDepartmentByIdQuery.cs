using EmployeeService.Application.DTOs.Departments;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Queries.GetDepartmentById;

public record GetDepartmentByIdQuery(int Id) : IRequest<DepartmentResponseDto?>;
