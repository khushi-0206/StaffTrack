using EmployeeService.Domain.Enums;

namespace EmployeeService.Application.DTOs.Employees;

public record CreateEmployeeRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    int DepartmentId,
    int RoleId,
    Guid? ManagerId,
    DateTime DateOfJoining,
    EmployeeStatus Status);

public record UpdateEmployeeRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    int DepartmentId,
    int RoleId,
    Guid? ManagerId,
    DateTime DateOfJoining,
    EmployeeStatus Status);

public record EmployeeResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    int DepartmentId,
    string DepartmentName,
    int RoleId,
    string RoleName,
    Guid? ManagerId,
    string? ManagerName,
    DateTime DateOfJoining,
    EmployeeStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record EmployeeListItemDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string DepartmentName,
    string RoleName,
    EmployeeStatus Status,
    DateTime DateOfJoining,
    DateTime CreatedAt);
