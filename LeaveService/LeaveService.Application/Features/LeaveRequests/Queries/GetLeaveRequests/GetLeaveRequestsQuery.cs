using LeaveService.Application.Common.Models;
using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Queries.GetLeaveRequests;

public record GetLeaveRequestsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? EmployeeId = null,
    LeaveRequestStatus? Status = null,
    int? LeaveTypeId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string? SortBy = null,
    bool SortDescending = false) : IRequest<PagedResult<LeaveRequestResponseDto>>;
