namespace EmployeeService.Application.DTOs.Holidays;

public record CreateHolidayRequestDto(string Name, DateOnly Date);

public record HolidayResponseDto(int Id, string Name, DateOnly Date, DateTime CreatedAt, DateTime? UpdatedAt);
