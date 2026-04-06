using LeaveService.Application.DTOs.Reports;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetYearlyLeaveReport;

public record GetYearlyLeaveReportQuery(int Year) : IRequest<YearlyLeaveReportDto>;
