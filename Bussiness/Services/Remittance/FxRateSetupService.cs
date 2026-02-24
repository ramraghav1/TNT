using System.Collections.Generic;
using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.FxRateSetup;

namespace Bussiness.Services.Remittance
{
    public interface IFxRateSetupService
    {
        FxRateDetailResponse Create(CreateFxRateRequest request);
        List<FxRateListItem> GetAll();
        FxRateDetailResponse? GetById(long id);
        FxRateDetailResponse? Update(long id, UpdateFxRateRequest request);
        bool Delete(long id);
        ConvertResponse? Convert(ConvertRequest request);
    }

    public class FxRateSetupService : IFxRateSetupService
    {
        private readonly IFxRateSetupRepository _repository;
        private readonly IMapper _mapper;

        public FxRateSetupService(IFxRateSetupRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public FxRateDetailResponse Create(CreateFxRateRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.FxRateSetupDTO.CreateFxRateRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<FxRateDetailResponse>(result);
        }

        public List<FxRateListItem> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<FxRateListItem>>(result);
        }

        public FxRateDetailResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<FxRateDetailResponse>(result);
        }

        public FxRateDetailResponse? Update(long id, UpdateFxRateRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.FxRateSetupDTO.UpdateFxRateRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<FxRateDetailResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }

        public ConvertResponse? Convert(ConvertRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.FxRateSetupDTO.ConvertRequest>(request);
            var result = _repository.Convert(dto);
            return result == null ? null : _mapper.Map<ConvertResponse>(result);
        }
    }
}
