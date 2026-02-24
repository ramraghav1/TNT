using AutoMapper;
using Domain.Models;
using Repository.DataModels;

namespace Bussiness.MappingProfiles
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // Map DB validated user response → Domain ValidatedUser
            CreateMap<AuthTokenDTO.ValidatedUserResponse, Auth.ValidatedUser>()
                .ForMember(dest => dest.LoginId, opt => opt.MapFrom(src => src.LoginId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.UserFullName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber));

            // Map DB token record → Domain token record (if needed in future)
            CreateMap<AuthTokenDTO.AuthTokenResponse, Auth.AuthTokenRecord>().ReverseMap();
        }
    }
}
