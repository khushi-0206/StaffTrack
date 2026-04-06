using TimeSheetService.Application.DTOs.Reports;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetMonthlyTimesheetReport;

public record GetMonthlyTimesheetReportQuery(int Year, int Month) : IRequest<MonthlyReportDto>;
