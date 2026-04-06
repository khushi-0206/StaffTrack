using LeaveService.Application.DTOs.LeaveTypes;
using MediatR;

namespace LeaveService.Application.Features.LeaveTypes.Commands.CreateLeaveType;

public record CreateLeaveTypeCommand(CreateLeaveTypeRequestDto Dto) : IRequest<int>;
