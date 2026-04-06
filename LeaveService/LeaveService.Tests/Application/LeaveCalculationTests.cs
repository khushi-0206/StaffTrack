using LeaveService.Application.Common;

namespace LeaveService.Tests.Application;

[TestFixture]
public class LeaveCalculationTests
{
    [Test]
    public void InclusiveCalendarDays_SingleDay_IsOne()
    {
        var d = new DateOnly(2026, 1, 1);
        Assert.That(LeaveCalculation.InclusiveCalendarDays(d, d), Is.EqualTo(1));
    }

    [Test]
    public void RangesOverlap_TouchingRanges_Overlap()
    {
        var aStart = new DateOnly(2026, 1, 1);
        var aEnd = new DateOnly(2026, 1, 5);
        var bStart = new DateOnly(2026, 1, 5);
        var bEnd = new DateOnly(2026, 1, 10);
        Assert.That(LeaveCalculation.RangesOverlap(aStart, aEnd, bStart, bEnd), Is.True);
    }
}
