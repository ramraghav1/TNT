using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.DataModels.TourAndTravels;

namespace Bussiness.MappingProfiles.TourAndTravels
{
    public class ItineraryProposalMappingProfile : Profile
    {
        public ItineraryProposalMappingProfile()
        {
            // Request: Domain to DTO
            CreateMap<ItineraryProposal.CreateProposalRequest, ItineraryProposalDTO.CreateProposalRequest>().ReverseMap();
            CreateMap<ItineraryProposal.ProposalDayRequest, ItineraryProposalDTO.ProposalDayRequest>().ReverseMap();
            CreateMap<ItineraryProposal.SubmitProposalPaymentRequest, ItineraryProposalDTO.SubmitProposalPaymentRequest>().ReverseMap();
            CreateMap<ItineraryProposal.SubmitFeedbackRequest, ItineraryProposalDTO.SubmitFeedbackRequest>().ReverseMap();

            // Response: DTO to Domain
            CreateMap<ItineraryProposalDTO.ProposalResponse, ItineraryProposal.ProposalResponse>().ReverseMap();
            CreateMap<ItineraryProposalDTO.ProposalDayResponse, ItineraryProposal.ProposalDayResponse>().ReverseMap();
            CreateMap<ItineraryProposalDTO.ProposalListItem, ItineraryProposal.ProposalListItem>().ReverseMap();
            CreateMap<ItineraryProposalDTO.ProposalPaymentResponse, ItineraryProposal.ProposalPaymentResponse>().ReverseMap();
            CreateMap<ItineraryProposalDTO.ProposalFeedbackResponse, ItineraryProposal.ProposalFeedbackResponse>().ReverseMap();
        }
    }
}
