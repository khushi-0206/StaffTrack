using TimeSheetService.Application.Common;
using TimeSheetService.Application.DTOs.Reports;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetProductivityReport;

/// <summary>Approved hours per employee in range (productivity proxy).</summary>
public class GetProductivityReportQueryHandler : IRequestHandler<GetProductivityReportQuery, ProductivityReportDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public GetProductivityReportQueryHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<ProductivityReportDto> Handle(
        GetProductivityReportQuery request,
        CancellationToken cancellationToken)
    {
        var page = await _uow.Timesheets.SearchAsync(
            1, 200_000, null, TimesheetStatus.Approved, request.From, request.To, null, false, cancellationToken);

        var items = page.Items.Where(x => !x.IsDeleted && x.Date >= request.From && x.Date <= request.To).ToList();

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

        return new ProductivityReportDto(request.From, request.To, byEmp);
    }
}
