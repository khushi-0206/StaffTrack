using AutoMapper;
using LeaveService.Application.DTOs.Reports;
using LeaveService.Application.Features.LeaveRequests;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetEmployeeLeaveReport;

public class GetEmployeeLeaveReportQueryHandler : IRequestHandler<GetEmployeeLeaveReportQuery, EmployeeLeaveReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetEmployeeLeaveReportQueryHandler(
        IUnitOfWork unitOfWork,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _employees = employees;
        _current = current;
        _mapper = mapper;
    }

    public async Task<EmployeeLeaveReportDto> Handle(
        GetEmployeeLeaveReportQuery request,
        CancellationToken cancellationToken)
    {
        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        var team = await _employees.GetEmployeeIdsForManagerAsync(callerEmployeeId, cancellationToken);

        LeaveRequestAuthorization.EnsureCanViewEmployeeLeaves(
            _current, request.EmployeeId, callerEmployeeId, team);

        var requests = await _unitOfWork.LeaveRequests.GetByEmployeeAsync(request.EmployeeId, cancellationToken);
        var list = requests.Where(r => !r.IsDeleted).ToList();

        var recent = list.OrderByDescending(r => r.AppliedAt).Take(20).ToList();

        return new EmployeeLeaveReportDto(
            request.EmployeeId,
            list.Count,
            list.Count(r => r.Status == LeaveRequestStatus.Pending),
            list.Count(r => r.Status == LeaveRequestStatus.Approved),
            list.Count(r => r.Status == LeaveRequestStatus.Rejected),
            list.Count(r => r.Status == LeaveRequestStatus.Cancelled),
            _mapper.Map<IReadOnlyList<LeaveRequestSummaryDto>>(recent));
    }
}
