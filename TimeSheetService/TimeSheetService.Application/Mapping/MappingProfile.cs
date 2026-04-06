using AutoMapper;
using TimeSheetService.Application.DTOs.Attendance;
using TimeSheetService.Application.DTOs.Projects;
using TimeSheetService.Application.DTOs.Reports;
using TimeSheetService.Application.DTOs.TimeEntries;
using TimeSheetService.Application.DTOs.Timesheets;
using TimeSheetService.Domain.Entities;

namespace TimeSheetService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Timesheet, TimesheetResponseDto>();

        CreateMap<Timesheet, TimesheetSummaryDto>()
            .ForMember(d => d.StatusLabel, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<TimeEntry, TimeEntryResponseDto>();
        CreateMap<Attendance, AttendanceResponseDto>();
        CreateMap<Project, ProjectResponseDto>();
    }
}
