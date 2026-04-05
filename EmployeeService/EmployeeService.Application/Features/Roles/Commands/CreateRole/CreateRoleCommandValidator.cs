using FluentValidation;

namespace EmployeeService.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(64);
    }
}
