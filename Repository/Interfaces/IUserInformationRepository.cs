using System;
using Repository.DataModels;

namespace Repository.Interfaces
{
	public interface IUserInformationRepository
	{
        int AddUserInformation(UserInformationDTO user);
        void UpdateUserInformation(UserInformationDTO user);
        List<UserInformationDTO> GetAllUsers();

    }
}

