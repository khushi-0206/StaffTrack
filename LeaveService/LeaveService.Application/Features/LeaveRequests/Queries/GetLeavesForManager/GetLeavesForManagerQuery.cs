using LeaveService.Application.DTOs.LeaveRequests;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Queries.GetLeavesForManager;

public record GetLeavesForManagerQuery(Guid ManagerId) : IRequest<IReadOnlyList<LeaveRequestResponseDto>>;
