using Domain.Models;
using AutoMapper;
using Repository.DataModels;

namespace Bussiness.MappingProfiles
{
	public class LoggedInUserInformationProfile : Profile
    {
		public LoggedInUserInformationProfile()
		{
            CreateMap<LoggedInUserInformationDTO, LoggedInUserInformation>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.userid))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.userfullname))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.emailaddress))
            .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.mobilenumber))
            .ReverseMap();

        }
	}
}

