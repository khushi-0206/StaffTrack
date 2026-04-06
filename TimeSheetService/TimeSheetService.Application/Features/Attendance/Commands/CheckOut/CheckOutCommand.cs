using TimeSheetService.Application.DTOs.Attendance;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Commands.CheckOut;

public record CheckOutCommand(CheckOutRequestDto Dto) : IRequest<Unit>;
