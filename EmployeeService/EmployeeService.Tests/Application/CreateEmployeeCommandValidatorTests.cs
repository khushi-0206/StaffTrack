using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Features.Employees.Commands.CreateEmployee;
using EmployeeService.Domain.Enums;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class CreateEmployeeCommandValidatorTests
{
    private CreateEmployeeCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateEmployeeCommandValidator();

    [Test]
    public void Validate_InvalidEmail_Fails()
    {
        var cmd = new CreateEmployeeCommand(new CreateEmployeeRequestDto(
            "Jane",
            "Doe",
            "not-an-email",
            null,
            1,
            1,
            null,
            DateTime.UtcNow.AddDays(-30),
            EmployeeStatus.Active));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_EmptyFirstName_Fails()
    {
        var cmd = new CreateEmployeeCommand(new CreateEmployeeRequestDto(
            "",
            "Doe",
            "jane@company.test",
            null,
            1,
            1,
            null,
            DateTime.UtcNow.AddDays(-30),
            EmployeeStatus.Active));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_DepartmentIdZero_Fails()
    {
        var cmd = new CreateEmployeeCommand(new CreateEmployeeRequestDto(
            "Jane",
            "Doe",
            "jane@company.test",
            null,
            0,
            1,
            null,
            DateTime.UtcNow.AddDays(-30),
            EmployeeStatus.Active));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidRequest_Succeeds()
    {
        var cmd = new CreateEmployeeCommand(new CreateEmployeeRequestDto(
            "Jane",
            "Doe",
            "jane.doe@company.test",
            null,
            1,
            1,
            null,
            DateTime.UtcNow.AddDays(-30),
            EmployeeStatus.Active));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.True);
    }
}
