using EmployeeService.Application.DTOs.Employees;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeesByManager;

public record GetEmployeesByManagerQuery(Guid ManagerId) : IRequest<IReadOnlyList<EmployeeListItemDto>>;
