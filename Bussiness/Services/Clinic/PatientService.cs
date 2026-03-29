using AutoMapper;
using Repository.Interfaces.Clinic;
using static Domain.Models.Clinic.Patient;

namespace Bussiness.Services.Clinic
{
    public interface IPatientService
    {
        PatientResponse Create(CreatePatientRequest request);
        List<PatientResponse> GetByTenant(long tenantId);
        PatientResponse? GetById(long tenantId, long id);
        PatientResponse? Update(long tenantId, long id, UpdatePatientRequest request);
        bool Delete(long tenantId, long id);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PatientResponse Create(CreatePatientRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.PatientDTO.CreatePatientRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<PatientResponse>(result);
        }

        public List<PatientResponse> GetByTenant(long tenantId)
        {
            var result = _repository.GetByTenant(tenantId);
            return _mapper.Map<List<PatientResponse>>(result);
        }

        public PatientResponse? GetById(long tenantId, long id)
        {
            var result = _repository.GetById(tenantId, id);
            return result == null ? null : _mapper.Map<PatientResponse>(result);
        }

        public PatientResponse? Update(long tenantId, long id, UpdatePatientRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.PatientDTO.UpdatePatientRequest>(request);
            var result = _repository.Update(tenantId, id, dto);
            return result == null ? null : _mapper.Map<PatientResponse>(result);
        }

        public bool Delete(long tenantId, long id)
        {
            return _repository.Delete(tenantId, id);
        }
    }
}
