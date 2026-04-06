using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;

namespace TimeSheetService.Infrastructure.Persistence.Repositories;

public class TimesheetHistoryRepository : ITimesheetHistoryRepository
{
    private readonly TimeSheetDbContext _context;

    public TimesheetHistoryRepository(TimeSheetDbContext context) => _context = context;

    public void Add(TimesheetHistory history) => _context.TimesheetHistories.Add(history);
}
