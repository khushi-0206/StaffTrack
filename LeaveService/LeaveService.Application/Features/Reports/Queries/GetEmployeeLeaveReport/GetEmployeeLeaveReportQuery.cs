using LeaveService.Application.DTOs.Reports;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetEmployeeLeaveReport;

public record GetEmployeeLeaveReportQuery(Guid EmployeeId) : IRequest<EmployeeLeaveReportDto>;
