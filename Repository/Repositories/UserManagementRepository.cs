using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using static Repository.DataModels.UserManagementDTO;

namespace Repository.Repositories
{
    public interface IUserManagementRepository
    {
        int CreateUser(string fullName, string email, string? mobile, string? address, long? tenantId, long? orgId, long? branchId, int createdBy);
        int CreateLoginDetail(int userId, string username, string passwordHash, long? tenantId, long? orgId);
        void AssignRolesToUser(int userId, List<long> roleIds, int assignedBy);
        List<UserListDTO> GetUsersByTenant(long tenantId);
        List<UserListDTO> GetUsersByOrganization(long orgId);
        void UpdateUserStatus(int userId, bool isActive);
    }

    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserManagementRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public int CreateUser(string fullName, string email, string? mobile, string? address, long? tenantId, long? orgId, long? branchId, int createdBy)
        {
            string sql = @"
                INSERT INTO userinformation (userfullname, emailaddress, mobilenumber, address, tenant_id, org_id, branch_id, createdby, createdat, updatedat)
                VALUES (@FullName, @Email, @Mobile, @Address, @TenantId, @OrgId, @BranchId, @CreatedBy, NOW(), NOW())
                RETURNING userid;";

            return _dbConnection.QuerySingle<int>(sql, new 
            { 
                FullName = fullName, 
                Email = email, 
                Mobile = mobile, 
                Address = address ?? "", 
                TenantId = tenantId, 
                OrgId = orgId, 
                BranchId = branchId, 
                CreatedBy = createdBy 
            });
        }

        public int CreateLoginDetail(int userId, string username, string passwordHash, long? tenantId, long? orgId)
        {
            string sql = @"
                INSERT INTO logindetail (userid, username, password, tenant_id, org_id, createdat, updatedat)
                VALUES (@UserId, @Username, @PasswordHash, @TenantId, @OrgId, NOW(), NOW())
                RETURNING id;";

            return _dbConnection.QuerySingle<int>(sql, new 
            { 
                UserId = userId, 
                Username = username, 
                PasswordHash = passwordHash, 
                TenantId = tenantId, 
                OrgId = orgId 
            });
        }

        public void AssignRolesToUser(int userId, List<long> roleIds, int assignedBy)
        {
            // First delete existing roles
            _dbConnection.Execute("DELETE FROM user_roles WHERE user_id = @UserId", new { UserId = userId });

            if (roleIds != null && roleIds.Any())
            {
                string sql = @"
                    INSERT INTO user_roles (user_id, role_id, assigned_by, assigned_at)
                    VALUES (@UserId, @RoleId, @AssignedBy, NOW())";

                foreach (var roleId in roleIds)
                {
                    _dbConnection.Execute(sql, new { UserId = userId, RoleId = roleId, AssignedBy = assignedBy });
                }
            }
        }

        public List<UserListDTO> GetUsersByTenant(long tenantId)
        {
            string sql = @"
                SELECT 
                    ui.userid AS UserId,
                    ui.userfullname AS UserFullName,
                    ui.emailaddress AS EmailAddress,
                    ui.mobilenumber AS MobileNumber,
                    ui.tenant_id AS TenantId,
                    st.name AS TenantName,
                    ui.org_id AS OrgId,
                    string_agg(r.display_name, ', ') AS Roles,
                    ui.createdat AS CreatedAt,
                    COALESCE(ld.id IS NOT NULL, false) AS IsActive
                FROM userinformation ui
                LEFT JOIN saas_tenants st ON st.id = ui.tenant_id
                LEFT JOIN logindetail ld ON ld.userid = ui.userid
                LEFT JOIN user_roles ur ON ur.user_id = ui.userid
                LEFT JOIN roles r ON r.id = ur.role_id
                WHERE ui.tenant_id = @TenantId
                GROUP BY ui.userid, ui.userfullname, ui.emailaddress, ui.mobilenumber, ui.tenant_id, st.name, ui.org_id, ui.createdat, ld.id
                ORDER BY ui.createdat DESC;";

            return _dbConnection.Query<UserListDTO>(sql, new { TenantId = tenantId }).ToList();
        }

        public List<UserListDTO> GetUsersByOrganization(long orgId)
        {
            string sql = @"
                SELECT 
                    ui.userid AS UserId,
                    ui.userfullname AS UserFullName,
                    ui.emailaddress AS EmailAddress,
                    ui.mobilenumber AS MobileNumber,
                    ui.tenant_id AS TenantId,
                    st.name AS TenantName,
                    ui.org_id AS OrgId,
                    string_agg(r.display_name, ', ') AS Roles,
                    ui.createdat AS CreatedAt,
                    COALESCE(ld.id IS NOT NULL, false) AS IsActive
                FROM userinformation ui
                LEFT JOIN saas_tenants st ON st.id = ui.tenant_id
                LEFT JOIN logindetail ld ON ld.userid = ui.userid
                LEFT JOIN user_roles ur ON ur.user_id = ui.userid
                LEFT JOIN roles r ON r.id = ur.role_id
                WHERE ui.org_id = @OrgId
                GROUP BY ui.userid, ui.userfullname, ui.emailaddress, ui.mobilenumber, ui.tenant_id, st.name, ui.org_id, ui.createdat, ld.id
                ORDER BY ui.createdat DESC;";

            return _dbConnection.Query<UserListDTO>(sql, new { OrgId = orgId }).ToList();
        }

        public void UpdateUserStatus(int userId, bool isActive)
        {
            if (isActive)
            {
                // Reactivate - ensure logindetail exists
                string checkSql = "SELECT COUNT(1) FROM logindetail WHERE userid = @UserId";
                int exists = _dbConnection.ExecuteScalar<int>(checkSql, new { UserId = userId });
                
                if (exists == 0)
                {
                    throw new InvalidOperationException("Cannot activate user without login credentials. Please create user first.");
                }
            }
            else
            {
                // Deactivate - delete logindetail
                _dbConnection.Execute("DELETE FROM logindetail WHERE userid = @UserId", new { UserId = userId });
            }
        }
    }
}
