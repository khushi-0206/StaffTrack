using FluentValidation;

namespace EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Dto.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Dto.Phone).MaximumLength(32);
        RuleFor(x => x.Dto.DepartmentId).GreaterThan(0);
        RuleFor(x => x.Dto.RoleId).GreaterThan(0);
    }
}
