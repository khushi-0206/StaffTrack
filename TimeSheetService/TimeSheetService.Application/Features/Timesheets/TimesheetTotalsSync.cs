using TimeSheetService.Application.Interfaces.Persistence;

namespace TimeSheetService.Application.Features.Timesheets;

internal static class TimesheetTotalsSync
{
    public static async Task RecalculateAsync(IUnitOfWork uow, Guid timesheetId, CancellationToken cancellationToken)
    {
        var sum = await uow.TimeEntries.SumHoursForTimesheetAsync(timesheetId, cancellationToken);
        var ts = await uow.Timesheets.GetByIdAsync(timesheetId, track: true, cancellationToken);
        if (ts is null || ts.IsDeleted)
            return;
        ts.TotalHours = sum;
        uow.Timesheets.Update(ts);
    }
}
