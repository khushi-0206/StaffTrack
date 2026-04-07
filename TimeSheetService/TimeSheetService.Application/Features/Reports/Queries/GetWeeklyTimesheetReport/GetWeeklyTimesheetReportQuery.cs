using TimeSheetService.Application.DTOs.Reports;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetWeeklyTimesheetReport;

public record GetWeeklyTimesheetReportQuery(int Year, int Week) : IRequest<WeeklyReportDto>;
