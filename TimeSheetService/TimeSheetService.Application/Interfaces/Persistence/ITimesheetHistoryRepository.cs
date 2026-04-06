using TimeSheetService.Domain.Entities;

namespace TimeSheetService.Application.Interfaces.Persistence;

public interface ITimesheetHistoryRepository
{
    void Add(TimesheetHistory history);
}
