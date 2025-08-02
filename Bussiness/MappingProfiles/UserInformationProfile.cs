using Domain.Models;
using AutoMapper;
using Repository.DataModels;

public class UserInformationProfile : Profile
{
    public UserInformationProfile()
    {
        CreateMap<UserInformationDTO, UserInformation>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.userid))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.userfullname))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.emailaddress))
            .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.mobilenumber))
            .ReverseMap(); // Also maps UserInformation -> UserInformationDTO

        CreateMap<UserInformationDTO, InsertUserInformation>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.userfullname))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.emailaddress))
            .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.mobilenumber))
            .ReverseMap();
    }
}
