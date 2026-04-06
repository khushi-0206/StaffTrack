using FluentValidation;

namespace LeaveService.Application.Features.LeaveRequests.Commands.ApplyLeave;

public class ApplyLeaveCommandValidator : AbstractValidator<ApplyLeaveCommand>
{
    public ApplyLeaveCommandValidator()
    {
        RuleFor(x => x.Dto.EmployeeId).NotEmpty();
        RuleFor(x => x.Dto.LeaveTypeId).GreaterThan(0);
        RuleFor(x => x.Dto.StartDate).NotEmpty();
        RuleFor(x => x.Dto.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.Dto.StartDate);
        RuleFor(x => x.Dto.Reason).NotEmpty().MaximumLength(2000);
    }
}
