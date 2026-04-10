using AutoMapper;
using Domain.Models.TourAndTravels;
using static Domain.Models.TourAndTravels.DepartureManagement;
using RepDTOs = Repository.DataModels.TourAndTravels.DepartureManagementDTO;

namespace Bussiness.MappingProfiles.TourAndTravels
{
    public class DepartureManagementMappingProfile : Profile
    {
        public DepartureManagementMappingProfile()
        {
            // Request: Domain → Repo DTO
            CreateMap<CreateDepartureRequest, RepDTOs.CreateDepartureRequest>();
            CreateMap<AssignBookingsToDepartureRequest, RepDTOs.AssignBookingsToDepartureRequest>();

            // Response: Repo DTO → Domain
            CreateMap<RepDTOs.UnassignedBookingItem, UnassignedBookingItem>().ReverseMap();
            CreateMap<RepDTOs.DepartureDetailResponse, DepartureDetailResponse>().ReverseMap();
            CreateMap<RepDTOs.SuggestedDepartureItem, SuggestedDepartureItem>().ReverseMap();
            CreateMap<RepDTOs.TravelerNotificationInfo, TravelerNotificationInfo>().ReverseMap();
            CreateMap<RepDTOs.DepartureListItem, DepartureListItem>()
                .ForMember(d => d.ComputedStatus, opt => opt.Ignore());
        }
    }
}
