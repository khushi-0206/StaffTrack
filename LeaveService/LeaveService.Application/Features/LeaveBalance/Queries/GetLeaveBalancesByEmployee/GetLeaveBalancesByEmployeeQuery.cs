using LeaveService.Application.DTOs.LeaveBalance;
using MediatR;

namespace LeaveService.Application.Features.LeaveBalance.Queries.GetLeaveBalancesByEmployee;

public record GetLeaveBalancesByEmployeeQuery(Guid EmployeeId) : IRequest<IReadOnlyList<LeaveBalanceResponseDto>>;
