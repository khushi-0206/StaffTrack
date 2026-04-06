using LeaveService.Application.DTOs.LeaveRequests;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Commands.CancelLeave;

public record CancelLeaveCommand(Guid Id, CancelLeaveRequestDto Dto) : IRequest<Unit>;
