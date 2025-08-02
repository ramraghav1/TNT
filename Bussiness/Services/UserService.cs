using AutoMapper;
using Domain.Models; // For UserInformation
using Repository; // For IUserInformationRepository (adjust namespace as needed)
using Repository.DataModels;
using Repository.Interfaces;

namespace Bussiness.Services
{
    public interface IUserService
    {
        bool AddUser(InsertUserInformation user);
        void UpdateUser(UserInformation user);
        List<UserInformation> GetAllUserInformation();
    }
    public class UserService : IUserService
    {
        private readonly IUserInformationRepository _userRepo;
        private readonly IMapper _mapper;

        // Constructor injection for repository and mapper
        public UserService(IUserInformationRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public bool AddUser(InsertUserInformation user)
        {
            // Map domain model to DTO
            var dto = _mapper.Map<UserInformationDTO>(user);

            // Call repository method with DTO
            int userId = _userRepo.AddUserInformation(dto);
            return (userId > 0);
        }

        public void UpdateUser(UserInformation user)
        {
            // Map domain model to DTO
            var dto = _mapper.Map<UserInformationDTO>(user);

            // Call repository method with DTO
            //_userRepo.UpdateUserInformation(dto);
        }
        public List<UserInformation> GetAllUserInformation()
        {
            var userInfoList = _userRepo.GetAllUsers(); // List<UserInformationDTO>
            var domainUsers = _mapper.Map<List<UserInformation>>(userInfoList);
            return domainUsers;
        }
    }
}
