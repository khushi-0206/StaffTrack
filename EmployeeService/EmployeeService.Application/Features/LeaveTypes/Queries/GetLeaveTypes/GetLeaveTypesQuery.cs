using EmployeeService.Application.DTOs.LeaveTypes;
using MediatR;

namespace EmployeeService.Application.Features.LeaveTypes.Queries.GetLeaveTypes;

public record GetLeaveTypesQuery : IRequest<IReadOnlyList<LeaveTypeResponseDto>>;
