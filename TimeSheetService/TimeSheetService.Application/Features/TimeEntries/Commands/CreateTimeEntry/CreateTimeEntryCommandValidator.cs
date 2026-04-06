using FluentValidation;

namespace TimeSheetService.Application.Features.TimeEntries.Commands.CreateTimeEntry;

public class CreateTimeEntryCommandValidator : AbstractValidator<CreateTimeEntryCommand>
{
    public CreateTimeEntryCommandValidator()
    {
        RuleFor(x => x.Dto.TimesheetId).NotEmpty();
        RuleFor(x => x.Dto.ProjectName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.TaskDescription).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Dto.HoursWorked).GreaterThan(0).LessThanOrEqualTo(24);
    }
}
