using EmployeeService.Application.DTOs.Employees;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeeSelf;

/// <summary>Resolves the Employee profile for the JWT caller (by email claim).</summary>
public record GetEmployeeSelfQuery : IRequest<EmployeeResponseDto?>;
