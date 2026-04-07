using MediatR;

namespace TimeSheetService.Application.Features.TimeEntries.Commands.DeleteTimeEntry;

public record DeleteTimeEntryCommand(Guid Id) : IRequest<Unit>;
