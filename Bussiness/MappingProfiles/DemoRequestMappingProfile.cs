using AutoMapper;
using Domain.Models;
using Repository.DataModels;

namespace Bussiness.MappingProfiles
{
    public class DemoRequestMappingProfile : Profile
    {
        public DemoRequestMappingProfile()
        {
            CreateMap<DemoRequest, DemoRequestDTO>();
        }
    }
}
