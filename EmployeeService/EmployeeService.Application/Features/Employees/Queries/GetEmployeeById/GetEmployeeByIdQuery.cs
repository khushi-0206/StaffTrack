using EmployeeService.Application.DTOs.Employees;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeResponseDto?>;
