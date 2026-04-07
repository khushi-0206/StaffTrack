using AutoMapper;
using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Common.Models;
using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Queries.GetLeaveRequests;

public class GetLeaveRequestsQueryHandler
    : IRequestHandler<GetLeaveRequestsQuery, PagedResult<LeaveRequestResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetLeaveRequestsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService current,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _current = current;
        _mapper = mapper;
    }

    public async Task<PagedResult<LeaveRequestResponseDto>> Handle(
        GetLeaveRequestsQuery request,
        CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException("Only HR or administrators can list all leave requests.");

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var result = await _unitOfWork.LeaveRequests.SearchAsync(
            page,
            pageSize,
            request.EmployeeId,
            request.Status,
            request.LeaveTypeId,
            request.FromDate,
            request.ToDate,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        var items = _mapper.Map<IReadOnlyList<LeaveRequestResponseDto>>(result.Items);
        return new PagedResult<LeaveRequestResponseDto>(items, page, pageSize, result.TotalCount);
    }
}
