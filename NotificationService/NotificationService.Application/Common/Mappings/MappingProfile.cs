using NotificationService.Application.Common.DTOs;
using NotificationService.Domain.Entities;
using AutoMapper;

namespace NotificationService.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<NotificationTemplate, NotificationTemplateDto>().ReverseMap();
        }
    }
}
