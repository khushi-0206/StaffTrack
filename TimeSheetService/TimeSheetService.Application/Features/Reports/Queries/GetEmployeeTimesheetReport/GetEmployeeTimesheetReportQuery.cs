using TimeSheetService.Application.DTOs.Reports;
using MediatR;

namespace TimeSheetService.Application.Features.Reports.Queries.GetEmployeeTimesheetReport;

public record GetEmployeeTimesheetReportQuery(Guid EmployeeId) : IRequest<EmployeeTimesheetReportDto>;
