using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Application.Features.LeaveRequests.Commands.ApplyLeave;

namespace LeaveService.Tests.Application;

[TestFixture]
public class ApplyLeaveCommandValidatorTests
{
    private ApplyLeaveCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new ApplyLeaveCommandValidator();

    [Test]
    public void Validate_EndBeforeStart_Fails()
    {
        var cmd = new ApplyLeaveCommand(new ApplyLeaveRequestDto(
            Guid.NewGuid(),
            1,
            new DateOnly(2026, 5, 10),
            new DateOnly(2026, 5, 1),
            "Holiday"));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidRequest_Succeeds()
    {
        var cmd = new ApplyLeaveCommand(new ApplyLeaveRequestDto(
            Guid.NewGuid(),
            1,
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 5),
            "Holiday"));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.True);
    }
}
