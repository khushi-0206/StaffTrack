using EmployeeService.Application.DTOs.LeaveTypes;
using MediatR;

namespace EmployeeService.Application.Features.LeaveTypes.Commands.CreateLeaveType;

public record CreateLeaveTypeCommand(CreateLeaveTypeRequestDto Dto) : IRequest<int>;
