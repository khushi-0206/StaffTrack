using TimeSheetService.Application.DTOs.Attendance;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Commands.CheckIn;

public record CheckInCommand(CheckInRequestDto Dto) : IRequest<Guid>;
