using EmployeeService.Application.DTOs.Roles;
using MediatR;

namespace EmployeeService.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleCommand(CreateRoleRequestDto Dto) : IRequest<int>;
