using System;
using AutoMapper;
using Domain.Models.Transaction;
using Repository.DataModels.Transaction;

namespace Bussiness.MappingProfiles.Transaction
{
	public class TransactionDetailMappingProfile : Profile
    {
        public TransactionDetailMappingProfile()
        {
            CreateMap<TransactionDetailDTO, TransactionDetailModel>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.transaction_id))
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => src.reference_number))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.payment_type))
            .ForMember(dest => dest.PayoutLocation, opt => opt.MapFrom(src => src.payout_location))
            .ForMember(dest => dest.CollectedAmount, opt => opt.MapFrom(src => src.collected_amount))
            .ForMember(dest => dest.ServiceFee, opt => opt.MapFrom(src => src.service_fee))
            .ForMember(dest => dest.TransferAmount, opt => opt.MapFrom(src => src.transfer_amount))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.sender_name))
            .ForMember(dest => dest.SenderAddress, opt => opt.MapFrom(src => src.sender_address))
            .ForMember(dest => dest.SenderMobile, opt => opt.MapFrom(src => src.sender_mobile))
            .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.receiver_name))
            .ForMember(dest => dest.ReceiverAddress, opt => opt.MapFrom(src => src.receiver_address))
            .ForMember(dest => dest.ReceiverMobile, opt => opt.MapFrom(src => src.receiver_mobile))
            .ReverseMap();

        }
    }
}

