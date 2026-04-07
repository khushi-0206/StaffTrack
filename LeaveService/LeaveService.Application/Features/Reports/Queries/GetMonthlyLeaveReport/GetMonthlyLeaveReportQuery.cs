using LeaveService.Application.DTOs.Reports;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetMonthlyLeaveReport;

public record GetMonthlyLeaveReportQuery(int Year, int Month) : IRequest<MonthlyLeaveReportDto>;
