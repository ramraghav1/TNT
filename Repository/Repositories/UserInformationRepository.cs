using System;
using System.Data;
using Dapper;
using Repository.DataModels;
using Repository.Interfaces;

namespace Repository.Repositories
{
	public class UserInformationRepository: IUserInformationRepository
    {
        private readonly IDbConnection _dbConnection;
        public UserInformationRepository(IDbConnection dbConnection)
		{
            _dbConnection = dbConnection;
        }
        public void AddUserInformation(UserInformationDTO user)
        {
            var sql = @"
            INSERT INTO userinformation 
            (userfullname, address, emailaddress, mobilenumber, createdby, updatedby, createdat, updatedat)
            VALUES 
            (@UserFullName, @Address, @EmailAddress, @MobileNumber, @CreatedBy, @UpdatedBy, @CreatedAt, @UpdatedAt);";

            

            _dbConnection.Execute(sql, user);
        }

        public void UpdateUserInformation(UserInformationDTO user)
        {
            var sql = @"
            UPDATE userinformation SET 
                userfullname = @UserFullName,
                address = @Address,
                emailaddress = @EmailAddress,
                mobilenumber = @MobileNumber,
                updatedby = @UpdatedBy,
                updatedat = @UpdatedAt
            WHERE userid = @UserId;";

         

            _dbConnection.Execute(sql, user);
        }
    }
}

