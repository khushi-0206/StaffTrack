using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.SoftDeleteEmployee;

public record SoftDeleteEmployeeCommand(Guid Id) : IRequest<Unit>;
