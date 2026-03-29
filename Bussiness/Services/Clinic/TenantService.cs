using AutoMapper;
using Repository.Interfaces.Clinic;
using static Domain.Models.Clinic.Tenant;

namespace Bussiness.Services.Clinic
{
    public interface ITenantService
    {
        TenantResponse Create(CreateTenantRequest request);
        List<TenantResponse> GetAll();
        TenantResponse? GetById(long id);
        TenantResponse? GetBySlug(string slug);
        TenantResponse? Update(long id, UpdateTenantRequest request);
        bool Delete(long id);
    }

    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repository;
        private readonly IMapper _mapper;

        public TenantService(ITenantRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TenantResponse Create(CreateTenantRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.TenantDTO.CreateTenantRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<TenantResponse>(result);
        }

        public List<TenantResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<TenantResponse>>(result);
        }

        public TenantResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<TenantResponse>(result);
        }

        public TenantResponse? GetBySlug(string slug)
        {
            var result = _repository.GetBySlug(slug);
            return result == null ? null : _mapper.Map<TenantResponse>(result);
        }

        public TenantResponse? Update(long id, UpdateTenantRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.TenantDTO.UpdateTenantRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<TenantResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
