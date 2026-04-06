using LeaveService.Application.DTOs.LeaveTypes;
using MediatR;

namespace LeaveService.Application.Features.LeaveTypes.Queries.GetLeaveTypes;

public record GetLeaveTypesQuery : IRequest<IReadOnlyList<LeaveTypeResponseDto>>;
