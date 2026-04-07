using LeaveService.Application.DTOs.LeaveRequests;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Queries.GetLeavesByEmployee;

public record GetLeavesByEmployeeQuery(Guid EmployeeId) : IRequest<IReadOnlyList<LeaveRequestResponseDto>>;
