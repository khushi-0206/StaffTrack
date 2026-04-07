using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;
using EmployeeService.Domain.Enums;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class UpdateEmployeeCommandValidatorTests
{
    private UpdateEmployeeCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new UpdateEmployeeCommandValidator();

    [Test]
    public void Validate_EmptyGuid_Fails()
    {
        var cmd = new UpdateEmployeeCommand(Guid.Empty, ValidDto());

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_InvalidEmail_Fails()
    {
        var dto = ValidDto() with { Email = "bad" };
        var cmd = new UpdateEmployeeCommand(Guid.NewGuid(), dto);

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidRequest_Succeeds()
    {
        var cmd = new UpdateEmployeeCommand(Guid.NewGuid(), ValidDto());

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.True);
    }

    private static UpdateEmployeeRequestDto ValidDto() => new(
        "Jane",
        "Doe",
        "jane@company.test",
        null,
        1,
        1,
        null,
        DateTime.UtcNow.AddDays(-30),
        EmployeeStatus.Active);
}
