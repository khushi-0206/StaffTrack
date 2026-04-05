using AutoMapper;
using EmployeeService.Application.DTOs.Departments;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.DTOs.Holidays;
using EmployeeService.Application.DTOs.LeaveTypes;
using EmployeeService.Application.DTOs.Roles;
using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Department, DepartmentResponseDto>();
        CreateMap<Role, RoleResponseDto>();
        CreateMap<LeaveType, LeaveTypeResponseDto>();
        CreateMap<Holiday, HolidayResponseDto>();

        CreateMap<Employee, EmployeeListItemDto>()
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.Name))
            .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role.Name));

        CreateMap<Employee, EmployeeResponseDto>()
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.Name))
            .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role.Name))
            .ForMember(d => d.ManagerName, o => o.MapFrom(s =>
                s.Manager == null ? null : $"{s.Manager.FirstName} {s.Manager.LastName}"));
    }
}
