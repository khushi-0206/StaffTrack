using EmployeeService.Application.DTOs.Departments;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequestDto Dto) : IRequest<int>;
