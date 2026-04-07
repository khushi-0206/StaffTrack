using LeaveService.Application.DTOs.LeaveBalance;
using MediatR;

namespace LeaveService.Application.Features.LeaveBalance.Commands.UpdateLeaveBalance;

public record UpdateLeaveBalanceCommand(UpdateLeaveBalanceRequestDto Dto) : IRequest<Unit>;
