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
        public int AddUserInformation(UserInformationDTO user)
        {
            if (_dbConnection.State != System.Data.ConnectionState.Open)
                _dbConnection.Open(); 

            using (var transaction = _dbConnection.BeginTransaction())
            {
                int userId = 0;
                try
                {
                    var insertUserSql = @"
                INSERT INTO userinformation 
                (userfullname, address, emailaddress, mobilenumber)
                VALUES 
                (@UserFullName, @Address, @EmailAddress, @MobileNumber)
                RETURNING userid;";

                    userId = _dbConnection.QuerySingle<int>(insertUserSql, user, transaction);

                    var insertLoginSql = @"
                INSERT INTO logindetail 
                (userid, username, password, createdat)
                VALUES 
                (@UserId, @Username, @PasswordHash, @CreatedAt);";

                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.emailaddress);
                    var loginParams = new
                    {
                        UserId = userId,
                        Username = user.emailaddress,
                        PasswordHash = passwordHash,
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbConnection.Execute(insertLoginSql, loginParams, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                return userId;
            }
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
        public List<UserInformationDTO> GetAllUsers()
        {
            string sql = "SELECT * FROM userinformation";
            return _dbConnection.Query<UserInformationDTO>(sql).AsList();
        }
    }
}

