namespace EmployeeService.Application.DTOs.Departments;

public record CreateDepartmentRequestDto(string Name, string? Description);

public record UpdateDepartmentRequestDto(string Name, string? Description);

public record DepartmentResponseDto(int Id, string Name, string? Description, DateTime CreatedAt, DateTime? UpdatedAt);
