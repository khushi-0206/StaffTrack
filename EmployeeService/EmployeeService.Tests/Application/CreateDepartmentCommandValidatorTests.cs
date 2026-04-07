using EmployeeService.Application.DTOs.Departments;
using EmployeeService.Application.Features.Departments.Commands.CreateDepartment;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class CreateDepartmentCommandValidatorTests
{
    private CreateDepartmentCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateDepartmentCommandValidator();

    [Test]
    public void Validate_EmptyName_Fails()
    {
        var cmd = new CreateDepartmentCommand(new CreateDepartmentRequestDto("", null));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_NameTooLong_Fails()
    {
        var cmd = new CreateDepartmentCommand(new CreateDepartmentRequestDto(new string('x', 201), null));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidRequest_Succeeds()
    {
        var cmd = new CreateDepartmentCommand(new CreateDepartmentRequestDto("HR", "Human resources"));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.True);
    }
}
