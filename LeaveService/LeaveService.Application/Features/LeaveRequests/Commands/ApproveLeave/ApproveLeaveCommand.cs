using LeaveService.Application.DTOs.LeaveRequests;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Commands.ApproveLeave;

public record ApproveLeaveCommand(Guid Id, ApproveLeaveRequestDto Dto) : IRequest<Unit>;
