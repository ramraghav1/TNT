using AutoMapper;
using Domain.Models.Remittance;
using Repository.DataModels.Remittance;

namespace Bussiness.MappingProfiles.Remittance
{
    public class RemittanceMappingProfile : Profile
    {
        public RemittanceMappingProfile()
        {
            // Country
            CreateMap<Country.CreateCountryRequest, CountryDTO.CreateCountryRequest>().ReverseMap();
            CreateMap<Country.UpdateCountryRequest, CountryDTO.UpdateCountryRequest>().ReverseMap();
            CreateMap<CountryDTO.CountryResponse, Country.CountryResponse>().ReverseMap();

            // PaymentType
            CreateMap<PaymentType.CreatePaymentTypeRequest, PaymentTypeDTO.CreatePaymentTypeRequest>().ReverseMap();
            CreateMap<PaymentType.UpdatePaymentTypeRequest, PaymentTypeDTO.UpdatePaymentTypeRequest>().ReverseMap();
            CreateMap<PaymentTypeDTO.PaymentTypeResponse, PaymentType.PaymentTypeResponse>().ReverseMap();

            // Agent
            CreateMap<Agent.CreateAgentRequest, AgentDTO.CreateAgentRequest>().ReverseMap();
            CreateMap<Agent.UpdateAgentRequest, AgentDTO.UpdateAgentRequest>().ReverseMap();
            CreateMap<AgentDTO.AgentResponse, Agent.AgentResponse>().ReverseMap();

            // ServiceChargeSetup
            CreateMap<ServiceChargeSetup.CreateSetupRequest, ServiceChargeSetupDTO.CreateSetupRequest>().ReverseMap();
            CreateMap<ServiceChargeSetup.SlabRequest, ServiceChargeSetupDTO.SlabRequest>().ReverseMap();
            CreateMap<ServiceChargeSetup.UpdateSetupRequest, ServiceChargeSetupDTO.UpdateSetupRequest>().ReverseMap();
            CreateMap<ServiceChargeSetupDTO.SetupListItem, ServiceChargeSetup.SetupListItem>().ReverseMap();
            CreateMap<ServiceChargeSetupDTO.SetupDetailResponse, ServiceChargeSetup.SetupDetailResponse>().ReverseMap();
            CreateMap<ServiceChargeSetupDTO.SlabResponse, ServiceChargeSetup.SlabResponse>().ReverseMap();
            CreateMap<ServiceChargeSetup.CalculateChargeRequest, ServiceChargeSetupDTO.CalculateChargeRequest>().ReverseMap();
            CreateMap<ServiceChargeSetupDTO.CalculateChargeResponse, ServiceChargeSetup.CalculateChargeResponse>().ReverseMap();
        }
    }
}
