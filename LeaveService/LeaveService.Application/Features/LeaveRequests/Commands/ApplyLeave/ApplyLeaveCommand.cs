using LeaveService.Application.DTOs.LeaveRequests;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Commands.ApplyLeave;

public record ApplyLeaveCommand(ApplyLeaveRequestDto Dto) : IRequest<Guid>;
