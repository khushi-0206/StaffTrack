using TimeSheetService.Application.Common;
using TimeSheetService.Application.DTOs.Reports;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetMonthlyTimesheetReport;

public class GetMonthlyTimesheetReportQueryHandler : IRequestHandler<GetMonthlyTimesheetReportQuery, MonthlyReportDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public GetMonthlyTimesheetReportQueryHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<MonthlyReportDto> Handle(
        GetMonthlyTimesheetReportQuery request,
        CancellationToken cancellationToken)
    {
        var start = new DateOnly(request.Year, request.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var page = await _uow.Timesheets.SearchAsync(
            1, 100_000, null, TimesheetStatus.Approved, start, end, null, false, cancellationToken);

        var items = page.Items.Where(x => !x.IsDeleted && x.Date >= start && x.Date <= end).ToList();

        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles))
        {
            var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
            if (TimesheetRoles.IsManager(_current.Roles))
            {
                var team = await _employees.GetEmployeeIdsForManagerAsync(callerId, cancellationToken);
                var set = team.Append(callerId).ToHashSet();
                items = items.Where(x => set.Contains(x.EmployeeId)).ToList();
            }
            else
                items = items.Where(x => x.EmployeeId == callerId).ToList();
        }

        var byEmp = items
            .GroupBy(x => x.EmployeeId)
            .Select(g => new EmployeeHoursDto(g.Key, g.Sum(x => x.TotalHours), g.Count()))
            .OrderByDescending(x => x.TotalHours)
            .ToList();

        return new MonthlyReportDto(request.Year, request.Month, byEmp);
    }
}
