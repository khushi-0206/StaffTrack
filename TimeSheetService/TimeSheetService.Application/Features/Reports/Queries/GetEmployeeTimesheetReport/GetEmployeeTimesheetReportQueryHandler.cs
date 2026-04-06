using AutoMapper;
using TimeSheetService.Application.DTOs.Reports;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetEmployeeTimesheetReport;

public class GetEmployeeTimesheetReportQueryHandler
    : IRequestHandler<GetEmployeeTimesheetReportQuery, EmployeeTimesheetReportDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetEmployeeTimesheetReportQueryHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IMapper mapper)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
        _mapper = mapper;
    }

    public async Task<EmployeeTimesheetReportDto> Handle(
        GetEmployeeTimesheetReportQuery request,
        CancellationToken cancellationToken)
    {
        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        var team = await _employees.GetEmployeeIdsForManagerAsync(callerId, cancellationToken);
        TimesheetAuthorization.EnsureCanViewEmployeeTimesheets(_current, request.EmployeeId, callerId, team);

        var list = (await _uow.Timesheets.GetByEmployeeAsync(request.EmployeeId, cancellationToken))
            .Where(x => !x.IsDeleted).ToList();

        var submitted = list.Where(x => x.Status is TimesheetStatus.Submitted or TimesheetStatus.Approved or TimesheetStatus.Rejected)
            .Sum(x => x.TotalHours);
        var approved = list.Where(x => x.Status == TimesheetStatus.Approved).Sum(x => x.TotalHours);
        var recent = list.OrderByDescending(x => x.Date).Take(15).ToList();

        return new EmployeeTimesheetReportDto(
            request.EmployeeId,
            list.Count,
            submitted,
            approved,
            _mapper.Map<IReadOnlyList<TimesheetSummaryDto>>(recent));
    }
}
