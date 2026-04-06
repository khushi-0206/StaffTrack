namespace TimeSheetService.Application.DTOs.Projects;

public record CreateProjectRequestDto(string Name, string? Description);

public record UpdateProjectRequestDto(string Name, string? Description);

public record ProjectResponseDto(int Id, string Name, string? Description, DateTime CreatedAt, DateTime? UpdatedAt);
