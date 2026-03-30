using AutoMapper;
using Repository.Interfaces.Clinic;
using static Domain.Models.Clinic.Practitioner;

namespace Bussiness.Services.Clinic
{
    public interface IPractitionerService
    {
        PractitionerResponse Create(CreatePractitionerRequest request);
        List<PractitionerResponse> GetByTenant(long tenantId);
        PractitionerResponse? GetById(long tenantId, long id);
        PractitionerResponse? Update(long tenantId, long id, UpdatePractitionerRequest request);
        bool Delete(long tenantId, long id);
    }

    public class PractitionerService : IPractitionerService
    {
        private readonly IPractitionerRepository _repository;
        private readonly IMapper _mapper;

        public PractitionerService(IPractitionerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PractitionerResponse Create(CreatePractitionerRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.PractitionerDTO.CreatePractitionerRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<PractitionerResponse>(result);
        }

        public List<PractitionerResponse> GetByTenant(long tenantId)
        {
            var result = _repository.GetByTenant(tenantId);
            return _mapper.Map<List<PractitionerResponse>>(result);
        }

        public PractitionerResponse? GetById(long tenantId, long id)
        {
            var result = _repository.GetById(tenantId, id);
            return result == null ? null : _mapper.Map<PractitionerResponse>(result);
        }

        public PractitionerResponse? Update(long tenantId, long id, UpdatePractitionerRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.PractitionerDTO.UpdatePractitionerRequest>(request);
            var result = _repository.Update(tenantId, id, dto);
            return result == null ? null : _mapper.Map<PractitionerResponse>(result);
        }

        public bool Delete(long tenantId, long id)
        {
            return _repository.Delete(tenantId, id);
        }
    }
}
