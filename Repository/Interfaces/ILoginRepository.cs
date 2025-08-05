using System;
using Repository.DataModels;

namespace Repository.Interfaces
{
	public interface ILoginRepository
	{
        LoggedInUserInformationDTO GetLoginUserInformation(string userName);
    }
}

