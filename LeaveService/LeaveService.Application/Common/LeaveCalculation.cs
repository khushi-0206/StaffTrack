namespace LeaveService.Application.Common;

public static class LeaveCalculation
{
    /// <summary>Inclusive calendar days between two dates.</summary>
    public static int InclusiveCalendarDays(DateOnly start, DateOnly end) =>
        end.DayNumber - start.DayNumber + 1;

    /// <summary>Whether two inclusive date ranges overlap.</summary>
    public static bool RangesOverlap(DateOnly aStart, DateOnly aEnd, DateOnly bStart, DateOnly bEnd) =>
        aStart <= bEnd && bStart <= aEnd;
}
