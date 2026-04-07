using AutoMapper;
using LeaveService.Application.DTOs.LeaveBalance;
using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Application.DTOs.LeaveTypes;
using LeaveService.Application.DTOs.Reports;
using LeaveService.Domain.Entities;

namespace LeaveService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LeaveType, LeaveTypeResponseDto>();

        CreateMap<LeaveBalance, LeaveBalanceResponseDto>()
            .ForMember(d => d.LeaveTypeName, o => o.MapFrom(s => s.LeaveType.Name));

        CreateMap<LeaveRequest, LeaveRequestResponseDto>()
            .ForMember(d => d.LeaveTypeName, o => o.MapFrom(s => s.LeaveType.Name));

        CreateMap<LeaveRequest, LeaveRequestSummaryDto>()
            .ForMember(d => d.LeaveTypeName, o => o.MapFrom(s => s.LeaveType.Name));
    }
}
