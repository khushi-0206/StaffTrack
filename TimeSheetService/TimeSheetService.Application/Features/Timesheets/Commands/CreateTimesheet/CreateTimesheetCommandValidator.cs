using FluentValidation;

namespace TimeSheetService.Application.Features.Timesheets.Commands.CreateTimesheet;

public class CreateTimesheetCommandValidator : AbstractValidator<CreateTimesheetCommand>
{
    public CreateTimesheetCommandValidator()
    {
        RuleFor(x => x.Dto.EmployeeId).NotEmpty();
        RuleFor(x => x.Dto.Date).NotEmpty();
    }
}
