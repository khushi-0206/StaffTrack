using EmployeeService.Application.DTOs.Holidays;
using MediatR;

namespace EmployeeService.Application.Features.Holidays.Queries.GetHolidays;

public record GetHolidaysQuery : IRequest<IReadOnlyList<HolidayResponseDto>>;
