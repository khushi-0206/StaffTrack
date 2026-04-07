using TimeSheetService.Application.DTOs.TimeEntries;
using MediatR;

namespace TimeSheetService.Application.Features.TimeEntries.Commands.UpdateTimeEntry;

public record UpdateTimeEntryCommand(Guid Id, UpdateTimeEntryRequestDto Dto) : IRequest<Unit>;
