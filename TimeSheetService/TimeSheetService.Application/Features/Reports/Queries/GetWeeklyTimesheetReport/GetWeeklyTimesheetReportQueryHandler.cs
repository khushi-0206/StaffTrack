using System.Globalization;
using TimeSheetService.Application.Common;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.DTOs.Reports;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetWeeklyTimesheetReport;

public class GetWeeklyTimesheetReportQueryHandler : IRequestHandler<GetWeeklyTimesheetReportQuery, WeeklyReportDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public GetWeeklyTimesheetReportQueryHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<WeeklyReportDto> Handle(
        GetWeeklyTimesheetReportQuery request,
        CancellationToken cancellationToken)
    {
        var monday = DateOnly.FromDateTime(ISOWeek.ToDateTime(request.Year, request.Week, DayOfWeek.Monday));
        var sunday = monday.AddDays(6);

        var page = await _uow.Timesheets.SearchAsync(
            1, 50_000, null, TimesheetStatus.Approved, monday, sunday, null, false, cancellationToken);

        var items = page.Items.Where(x => !x.IsDeleted && x.Date >= monday && x.Date <= sunday).ToList();

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

        return new WeeklyReportDto(request.Year, request.Week, byEmp);
    }
}
