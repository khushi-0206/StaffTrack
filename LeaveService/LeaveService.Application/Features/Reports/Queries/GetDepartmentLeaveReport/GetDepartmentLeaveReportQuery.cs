using LeaveService.Application.DTOs.Reports;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetDepartmentLeaveReport;

public record GetDepartmentLeaveReportQuery(int DepartmentId, DateOnly? From, DateOnly? To)
    : IRequest<DepartmentLeaveReportDto>;
