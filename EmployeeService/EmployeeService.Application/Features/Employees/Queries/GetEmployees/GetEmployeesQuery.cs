using EmployeeService.Application.Common.Models;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Domain.Enums;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployees;

public record GetEmployeesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    int? DepartmentId = null,
    int? RoleId = null,
    EmployeeStatus? Status = null,
    string? SortBy = null,
    bool SortDescending = false) : IRequest<PagedResult<EmployeeListItemDto>>;
