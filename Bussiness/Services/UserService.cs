using AutoMapper;
using Domain.Models; // For UserInformation
using Repository; // For IUserInformationRepository (adjust namespace as needed)
using Repository.DataModels;
using Repository.Interfaces;

namespace Bussiness.Services
{
    public class UserService
    {
        private readonly IUserInformationRepository _userRepo;
        private readonly IMapper _mapper;

        // Constructor injection for repository and mapper
        public UserService(IUserInformationRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public void AddUser(UserInformation user)
        {
            // Map domain model to DTO
            var dto = _mapper.Map<UserInformationDTO>(user);

            // Call repository method with DTO
            _userRepo.u(dto);
        }

        public void UpdateUser(UserInformation user)
        {
            // Map domain model to DTO
            var dto = _mapper.Map<UserInformationDTO>(user);

            // Call repository method with DTO
            _userRepo.UpdateUserInformation(dto);
        }
    }
}
