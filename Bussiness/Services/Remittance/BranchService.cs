using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.Branch;

namespace Bussiness.Services.Remittance
{
    public interface IBranchService
    {
        BranchResponse Create(CreateBranchRequest request);
        List<BranchResponse> GetAll();
        List<BranchResponse> GetByAgent(long agentId);
        BranchResponse? GetById(long id);
        BranchResponse? Update(long id, UpdateBranchRequest request);
        bool Delete(long id);
    }

    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _repository;
        private readonly IMapper _mapper;

        public BranchService(IBranchRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public BranchResponse Create(CreateBranchRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.BranchDTO.CreateBranchRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<BranchResponse>(result);
        }

        public List<BranchResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<BranchResponse>>(result);
        }

        public List<BranchResponse> GetByAgent(long agentId)
        {
            var result = _repository.GetByAgent(agentId);
            return _mapper.Map<List<BranchResponse>>(result);
        }

        public BranchResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<BranchResponse>(result);
        }

        public BranchResponse? Update(long id, UpdateBranchRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.BranchDTO.UpdateBranchRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<BranchResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
