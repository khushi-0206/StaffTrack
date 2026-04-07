using EmployeeService.Application.DTOs.LeaveTypes;
using EmployeeService.Application.Features.LeaveTypes.Commands.CreateLeaveType;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class CreateLeaveTypeCommandValidatorTests
{
    private CreateLeaveTypeCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateLeaveTypeCommandValidator();

    [Test]
    public void Validate_NegativeMaxDays_Fails()
    {
        var cmd = new CreateLeaveTypeCommand(new CreateLeaveTypeRequestDto("Annual", -1));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_MaxDaysAbove366_Fails()
    {
        var cmd = new CreateLeaveTypeCommand(new CreateLeaveTypeRequestDto("Annual", 400));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidRequest_Succeeds()
    {
        var cmd = new CreateLeaveTypeCommand(new CreateLeaveTypeRequestDto("Annual", 25));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.True);
    }
}
