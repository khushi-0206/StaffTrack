using EmployeeService.Application.DTOs.Employees;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeRequestDto Dto) : IRequest<Unit>;
