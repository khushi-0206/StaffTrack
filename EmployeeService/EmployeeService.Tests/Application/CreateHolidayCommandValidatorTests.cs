using EmployeeService.Application.DTOs.Holidays;
using EmployeeService.Application.Features.Holidays.Commands.CreateHoliday;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class CreateHolidayCommandValidatorTests
{
    private CreateHolidayCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateHolidayCommandValidator();

    [Test]
    public void Validate_EmptyName_Fails()
    {
        var cmd = new CreateHolidayCommand(new CreateHolidayRequestDto("", new DateOnly(2026, 1, 1)));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidRequest_Succeeds()
    {
        var cmd = new CreateHolidayCommand(new CreateHolidayRequestDto("New Year", new DateOnly(2026, 1, 1)));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.True);
    }
}
