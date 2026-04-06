using LeaveService.Application.DTOs.LeaveTypes;
using MediatR;

namespace LeaveService.Application.Features.LeaveTypes.Commands.UpdateLeaveType;

public record UpdateLeaveTypeCommand(int Id, UpdateLeaveTypeRequestDto Dto) : IRequest<Unit>;
