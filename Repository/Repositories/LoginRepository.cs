using System;
using System.Data;
using Dapper;
using Repository.DataModels;
using Repository.Interfaces;

namespace Repository.Repositories
{
	public class LoginRepository: ILoginRepository
    {
        private readonly IDbConnection _dbConnection;
        public LoginRepository(IDbConnection dbConnection)
		{
            _dbConnection = dbConnection;
        }
        public LoggedInUserInformationDTO GetLoginUserInformation(string userName)
        {
            string sqlQuery = @"
                SELECT 
                    ld.username,
                    ui.userfullname,
                    ld.id AS loginid,
                    ui.userid,
                    ui.mobilenumber,
                    ui.emailaddress
                FROM 
                    userInformation ui 
                JOIN 
                    logindetail ld ON ui.userid = ld.userid
                WHERE 
                    ld.username = @Username;";

            var queryParam = new { Username = userName };

            return _dbConnection
                .Query<LoggedInUserInformationDTO>(sqlQuery, queryParam)
                .FirstOrDefault();
        }

    }
}

