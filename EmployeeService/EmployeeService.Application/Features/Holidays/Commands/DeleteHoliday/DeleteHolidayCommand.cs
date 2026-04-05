using MediatR;

namespace EmployeeService.Application.Features.Holidays.Commands.DeleteHoliday;

public record DeleteHolidayCommand(int Id) : IRequest<Unit>;
