using System.Collections.Generic;
using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.PaymentType;

namespace Bussiness.Services.Remittance
{
    public interface IPaymentTypeService
    {
        PaymentTypeResponse Create(CreatePaymentTypeRequest request);
        List<PaymentTypeResponse> GetAll();
        PaymentTypeResponse? GetById(long id);
        PaymentTypeResponse? Update(long id, UpdatePaymentTypeRequest request);
        bool Delete(long id);
    }

    public class PaymentTypeService : IPaymentTypeService
    {
        private readonly IPaymentTypeRepository _repository;
        private readonly IMapper _mapper;

        public PaymentTypeService(IPaymentTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PaymentTypeResponse Create(CreatePaymentTypeRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.PaymentTypeDTO.CreatePaymentTypeRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<PaymentTypeResponse>(result);
        }

        public List<PaymentTypeResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<PaymentTypeResponse>>(result);
        }

        public PaymentTypeResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<PaymentTypeResponse>(result);
        }

        public PaymentTypeResponse? Update(long id, UpdatePaymentTypeRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.PaymentTypeDTO.UpdatePaymentTypeRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<PaymentTypeResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
