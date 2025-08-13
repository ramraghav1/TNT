using System;
using AutoMapper;
using Domain.Models.Organization;
using Repository.DataModels.Organization;

namespace Bussiness.MappingProfiles.Organization
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<CreateOrganizationDTO, CreateOrganization>()
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.organization_name))
                .ForMember(dest => dest.OrganizationType, opt => opt.MapFrom(src => src.organization_type))
                .ForMember(dest => dest.CountryIso3, opt => opt.MapFrom(src => src.country_iso3))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status))
                .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.contact_person))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.contact_email))
                .ForMember(dest => dest.ContactPhone, opt => opt.MapFrom(src => src.contact_phone))
                .ReverseMap();


            CreateMap<OrganizationDetailDTO, OrganizationDetail>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.organization_id))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.organization_name))
                .ForMember(dest => dest.OrganizationType, opt => opt.MapFrom(src => src.organization_type))
                .ForMember(dest => dest.CountryIso3, opt => opt.MapFrom(src => src.country_iso3))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status))
                .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.contact_person))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.contact_email))
                .ForMember(dest => dest.ContactPhone, opt => opt.MapFrom(src => src.contact_phone))
                .ReverseMap();
        }
    }
}

