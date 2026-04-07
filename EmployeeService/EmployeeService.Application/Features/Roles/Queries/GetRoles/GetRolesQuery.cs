using EmployeeService.Application.DTOs.Roles;
using MediatR;

namespace EmployeeService.Application.Features.Roles.Queries.GetRoles;

public record GetRolesQuery : IRequest<IReadOnlyList<RoleResponseDto>>;
