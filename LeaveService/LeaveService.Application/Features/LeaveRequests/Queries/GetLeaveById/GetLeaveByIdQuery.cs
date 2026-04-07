using LeaveService.Application.DTOs.LeaveRequests;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Queries.GetLeaveById;

public record GetLeaveByIdQuery(Guid Id) : IRequest<LeaveRequestResponseDto?>;
