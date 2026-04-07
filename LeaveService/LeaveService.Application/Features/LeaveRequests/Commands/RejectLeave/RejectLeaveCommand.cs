using LeaveService.Application.DTOs.LeaveRequests;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Commands.RejectLeave;

public record RejectLeaveCommand(Guid Id, RejectLeaveRequestDto Dto) : IRequest<Unit>;
