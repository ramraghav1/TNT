using System.Collections.Generic;
using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.Country;

namespace Bussiness.Services.Remittance
{
    public interface ICountryService
    {
        CountryResponse Create(CreateCountryRequest request);
        List<CountryResponse> GetAll();
        CountryResponse? GetById(long id);
        CountryResponse? Update(long id, UpdateCountryRequest request);
        bool Delete(long id);
    }

    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repository;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public CountryResponse Create(CreateCountryRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.CountryDTO.CreateCountryRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<CountryResponse>(result);
        }

        public List<CountryResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<CountryResponse>>(result);
        }

        public CountryResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<CountryResponse>(result);
        }

        public CountryResponse? Update(long id, UpdateCountryRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.CountryDTO.UpdateCountryRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<CountryResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
