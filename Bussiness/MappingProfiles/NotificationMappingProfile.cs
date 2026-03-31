using AutoMapper;
using Domain.Models;
using Repository.DataModels;

namespace Bussiness.MappingProfiles
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<CreateNotification, CreateNotificationDTO>();
            CreateMap<NotificationDTO, NotificationItem>();
        }
    }
}
