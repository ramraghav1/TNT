using AutoMapper;
using Repository.Interfaces.Clinic;
using static Domain.Models.Clinic.ClinicService;

namespace Bussiness.Services.Clinic
{
    public interface IClinicServiceService
    {
        ClinicServiceResponse Create(CreateClinicServiceRequest request);
        List<ClinicServiceResponse> GetByTenant(long tenantId);
        ClinicServiceResponse? GetById(long tenantId, long id);
        ClinicServiceResponse? Update(long tenantId, long id, UpdateClinicServiceRequest request);
        bool Delete(long tenantId, long id);
    }

    public class ClinicServiceService : IClinicServiceService
    {
        private readonly IClinicServiceRepository _repository;
        private readonly IMapper _mapper;

        public ClinicServiceService(IClinicServiceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public ClinicServiceResponse Create(CreateClinicServiceRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.ClinicServiceDTO.CreateClinicServiceRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<ClinicServiceResponse>(result);
        }

        public List<ClinicServiceResponse> GetByTenant(long tenantId)
        {
            var result = _repository.GetByTenant(tenantId);
            return _mapper.Map<List<ClinicServiceResponse>>(result);
        }

        public ClinicServiceResponse? GetById(long tenantId, long id)
        {
            var result = _repository.GetById(tenantId, id);
            return result == null ? null : _mapper.Map<ClinicServiceResponse>(result);
        }

        public ClinicServiceResponse? Update(long tenantId, long id, UpdateClinicServiceRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.ClinicServiceDTO.UpdateClinicServiceRequest>(request);
            var result = _repository.Update(tenantId, id, dto);
            return result == null ? null : _mapper.Map<ClinicServiceResponse>(result);
        }

        public bool Delete(long tenantId, long id)
        {
            return _repository.Delete(tenantId, id);
        }
    }
}
