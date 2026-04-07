namespace EmployeeService.Application.DTOs.Roles;

public record CreateRoleRequestDto(string Name);

public record RoleResponseDto(int Id, string Name, DateTime CreatedAt, DateTime? UpdatedAt);
