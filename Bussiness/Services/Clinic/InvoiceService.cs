using AutoMapper;
using Repository.Interfaces.Clinic;
using static Domain.Models.Clinic.Invoice;

namespace Bussiness.Services.Clinic
{
    public interface IInvoiceService
    {
        InvoiceResponse Create(CreateInvoiceRequest request);
        List<InvoiceResponse> GetByTenant(long tenantId);
        List<InvoiceResponse> GetByPatient(long tenantId, long patientId);
        InvoiceResponse? GetById(long tenantId, long id);
        InvoiceResponse? Update(long tenantId, long id, UpdateInvoiceRequest request);
        bool Delete(long tenantId, long id);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IMapper _mapper;

        public InvoiceService(IInvoiceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public InvoiceResponse Create(CreateInvoiceRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.InvoiceDTO.CreateInvoiceRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<InvoiceResponse>(result);
        }

        public List<InvoiceResponse> GetByTenant(long tenantId)
        {
            var result = _repository.GetByTenant(tenantId);
            return _mapper.Map<List<InvoiceResponse>>(result);
        }

        public List<InvoiceResponse> GetByPatient(long tenantId, long patientId)
        {
            var result = _repository.GetByPatient(tenantId, patientId);
            return _mapper.Map<List<InvoiceResponse>>(result);
        }

        public InvoiceResponse? GetById(long tenantId, long id)
        {
            var result = _repository.GetById(tenantId, id);
            return result == null ? null : _mapper.Map<InvoiceResponse>(result);
        }

        public InvoiceResponse? Update(long tenantId, long id, UpdateInvoiceRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.InvoiceDTO.UpdateInvoiceRequest>(request);
            var result = _repository.Update(tenantId, id, dto);
            return result == null ? null : _mapper.Map<InvoiceResponse>(result);
        }

        public bool Delete(long tenantId, long id)
        {
            return _repository.Delete(tenantId, id);
        }
    }
}
