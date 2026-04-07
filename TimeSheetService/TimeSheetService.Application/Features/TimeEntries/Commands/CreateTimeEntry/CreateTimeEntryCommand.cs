using TimeSheetService.Application.DTOs.TimeEntries;
using MediatR;

namespace TimeSheetService.Application.Features.TimeEntries.Commands.CreateTimeEntry;

public record CreateTimeEntryCommand(CreateTimeEntryRequestDto Dto) : IRequest<Guid>;
