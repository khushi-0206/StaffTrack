using EmployeeService.Application.DTOs.Departments;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Commands.UpdateDepartment;

public record UpdateDepartmentCommand(int Id, UpdateDepartmentRequestDto Dto) : IRequest<Unit>;
