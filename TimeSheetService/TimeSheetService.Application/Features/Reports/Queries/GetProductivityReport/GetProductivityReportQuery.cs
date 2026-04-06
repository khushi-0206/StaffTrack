using TimeSheetService.Application.DTOs.Reports;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetProductivityReport;

public record GetProductivityReportQuery(DateOnly From, DateOnly To) : IRequest<ProductivityReportDto>;
