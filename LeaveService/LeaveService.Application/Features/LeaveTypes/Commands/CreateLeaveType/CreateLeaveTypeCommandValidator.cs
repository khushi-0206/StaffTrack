using FluentValidation;

namespace LeaveService.Application.Features.LeaveTypes.Commands.CreateLeaveType;

public class CreateLeaveTypeCommandValidator : AbstractValidator<CreateLeaveTypeCommand>
{
    public CreateLeaveTypeCommandValidator()
    {
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.MaxDays).GreaterThanOrEqualTo(0).LessThanOrEqualTo(366);
    }
}
