using System.Collections.Generic;
using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.AdministrativeDivision;

namespace Bussiness.Services.Remittance
{
    public interface IAdministrativeDivisionService
    {
        List<AdministrativeDivisionResponse> GetByCountry(long countryId);
        List<AdministrativeDivisionResponse> GetByCountryAndLevel(long countryId, int level);
        List<AdministrativeDivisionResponse> GetChildren(long parentId);
    }

    public class AdministrativeDivisionService : IAdministrativeDivisionService
    {
        private readonly IAdministrativeDivisionRepository _repository;
        private readonly IMapper _mapper;

        public AdministrativeDivisionService(IAdministrativeDivisionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<AdministrativeDivisionResponse> GetByCountry(long countryId)
        {
            var result = _repository.GetByCountry(countryId);
            return _mapper.Map<List<AdministrativeDivisionResponse>>(result);
        }

        public List<AdministrativeDivisionResponse> GetByCountryAndLevel(long countryId, int level)
        {
            var result = _repository.GetByCountryAndLevel(countryId, level);
            return _mapper.Map<List<AdministrativeDivisionResponse>>(result);
        }

        public List<AdministrativeDivisionResponse> GetChildren(long parentId)
        {
            var result = _repository.GetChildren(parentId);
            return _mapper.Map<List<AdministrativeDivisionResponse>>(result);
        }
    }
}
