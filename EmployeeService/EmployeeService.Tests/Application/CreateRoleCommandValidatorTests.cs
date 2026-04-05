using EmployeeService.Application.DTOs.Roles;
using EmployeeService.Application.Features.Roles.Commands.CreateRole;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class CreateRoleCommandValidatorTests
{
    private CreateRoleCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateRoleCommandValidator();

    [Test]
    public void Validate_EmptyName_Fails()
    {
        var cmd = new CreateRoleCommand(new CreateRoleRequestDto(""));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Validate_ValidRequest_Succeeds()
    {
        var cmd = new CreateRoleCommand(new CreateRoleRequestDto("Contractor"));

        var result = _validator.Validate(cmd);

        Assert.That(result.IsValid, Is.True);
    }
}
