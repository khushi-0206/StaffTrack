using EmployeeService.Application.DTOs.Holidays;
using MediatR;

namespace EmployeeService.Application.Features.Holidays.Commands.CreateHoliday;

public record CreateHolidayCommand(CreateHolidayRequestDto Dto) : IRequest<int>;
