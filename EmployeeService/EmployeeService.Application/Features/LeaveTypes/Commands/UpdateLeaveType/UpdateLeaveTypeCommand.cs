using EmployeeService.Application.DTOs.LeaveTypes;
using MediatR;

namespace EmployeeService.Application.Features.LeaveTypes.Commands.UpdateLeaveType;

public record UpdateLeaveTypeCommand(int Id, UpdateLeaveTypeRequestDto Dto) : IRequest<Unit>;
