using System.Collections.Generic;
using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.DomesticServiceChargeSetup;

namespace Bussiness.Services.Remittance
{
    public interface IDomesticServiceChargeSetupService
    {
        SetupDetailResponse Create(CreateSetupRequest request);
        List<SetupListItem> GetAll();
        SetupDetailResponse? GetById(long id);
        SetupDetailResponse? Update(long id, UpdateSetupRequest request);
        bool Delete(long id);
        CalculateChargeResponse? CalculateCharge(CalculateChargeRequest request);
    }

    public class DomesticServiceChargeSetupService : IDomesticServiceChargeSetupService
    {
        private readonly IDomesticServiceChargeSetupRepository _repository;
        private readonly IMapper _mapper;

        public DomesticServiceChargeSetupService(IDomesticServiceChargeSetupRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public SetupDetailResponse Create(CreateSetupRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.DomesticServiceChargeSetupDTO.CreateSetupRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<SetupDetailResponse>(result);
        }

        public List<SetupListItem> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<SetupListItem>>(result);
        }

        public SetupDetailResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<SetupDetailResponse>(result);
        }

        public SetupDetailResponse? Update(long id, UpdateSetupRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.DomesticServiceChargeSetupDTO.UpdateSetupRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<SetupDetailResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }

        public CalculateChargeResponse? CalculateCharge(CalculateChargeRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.DomesticServiceChargeSetupDTO.CalculateChargeRequest>(request);
            var result = _repository.CalculateCharge(dto);
            return result == null ? null : _mapper.Map<CalculateChargeResponse>(result);
        }
    }
}
