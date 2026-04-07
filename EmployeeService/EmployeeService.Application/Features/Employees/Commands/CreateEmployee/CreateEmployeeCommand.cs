using EmployeeService.Application.DTOs.Employees;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(CreateEmployeeRequestDto Dto) : IRequest<Guid>;
