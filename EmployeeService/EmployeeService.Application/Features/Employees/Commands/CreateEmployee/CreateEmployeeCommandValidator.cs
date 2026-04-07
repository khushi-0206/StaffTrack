using FluentValidation;

namespace EmployeeService.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.Dto.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Dto.Phone).MaximumLength(32);
        RuleFor(x => x.Dto.DepartmentId).GreaterThan(0);
        RuleFor(x => x.Dto.RoleId).GreaterThan(0);
        RuleFor(x => x.Dto.DateOfJoining).LessThanOrEqualTo(_ => DateTime.UtcNow.AddDays(1));
    }
}
