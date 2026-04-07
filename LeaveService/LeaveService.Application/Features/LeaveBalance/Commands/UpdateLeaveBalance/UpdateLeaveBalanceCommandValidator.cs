using FluentValidation;

namespace LeaveService.Application.Features.LeaveBalance.Commands.UpdateLeaveBalance;

public class UpdateLeaveBalanceCommandValidator : AbstractValidator<UpdateLeaveBalanceCommand>
{
    public UpdateLeaveBalanceCommandValidator()
    {
        RuleFor(x => x.Dto.EmployeeId).NotEmpty();
        RuleFor(x => x.Dto.LeaveTypeId).GreaterThan(0);
        RuleFor(x => x.Dto.TotalDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Dto.UsedDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Dto).Must(d => d.UsedDays <= d.TotalDays)
            .WithMessage("UsedDays cannot exceed TotalDays.");
    }
}
