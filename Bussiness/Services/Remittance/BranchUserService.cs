using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.BranchUser;

namespace Bussiness.Services.Remittance
{
    public interface IBranchUserService
    {
        BranchUserResponse Create(CreateBranchUserRequest request);
        List<BranchUserResponse> GetByBranch(long branchId);
        BranchUserResponse? GetById(long id);
        BranchUserResponse? Update(long id, UpdateBranchUserRequest request);
        bool Delete(long id);
    }

    public class BranchUserService : IBranchUserService
    {
        private readonly IBranchUserRepository _repository;
        private readonly IMapper _mapper;

        public BranchUserService(IBranchUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public BranchUserResponse Create(CreateBranchUserRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.BranchUserDTO.CreateBranchUserRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<BranchUserResponse>(result);
        }

        public List<BranchUserResponse> GetByBranch(long branchId)
        {
            var result = _repository.GetByBranch(branchId);
            return _mapper.Map<List<BranchUserResponse>>(result);
        }

        public BranchUserResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<BranchUserResponse>(result);
        }

        public BranchUserResponse? Update(long id, UpdateBranchUserRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.BranchUserDTO.UpdateBranchUserRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<BranchUserResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
