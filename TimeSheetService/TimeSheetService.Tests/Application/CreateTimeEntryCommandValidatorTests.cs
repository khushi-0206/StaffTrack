using TimeSheetService.Application.DTOs.TimeEntries;
using TimeSheetService.Application.Features.TimeEntries.Commands.CreateTimeEntry;

namespace TimeSheetService.Tests.Application;

[TestFixture]
public class CreateTimeEntryCommandValidatorTests
{
    private CreateTimeEntryCommandValidator _v = null!;

    [SetUp]
    public void SetUp() => _v = new CreateTimeEntryCommandValidator();

    [Test]
    public void Validate_HoursAbove24_Fails()
    {
        var cmd = new CreateTimeEntryCommand(new CreateTimeEntryRequestDto(
            Guid.NewGuid(), null, "P", "Task", 25m, new DateOnly(2026, 1, 1)));
        Assert.That(_v.Validate(cmd).IsValid, Is.False);
    }

    [Test]
    public void Validate_Valid_Succeeds()
    {
        var cmd = new CreateTimeEntryCommand(new CreateTimeEntryRequestDto(
            Guid.NewGuid(), null, "P", "Task", 4m, new DateOnly(2026, 1, 1)));
        Assert.That(_v.Validate(cmd).IsValid, Is.True);
    }
}
