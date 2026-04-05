using System.Collections.Generic;
using Repository.Repositories;
using static Repository.DataModels.UserManagementDTO;

namespace Bussiness.Services
{
    public interface IRolePermissionService
    {
        List<RoleDTO> GetRolesByTenantAndProduct(long tenantId, int productId);
        List<PermissionDTO> GetPermissionsByProduct(int productId);
        List<PermissionDTO> GetUserPermissions(int userId);
        long CreateRole(long tenantId, int productId, string name, string displayName, string? description);
        void UpdateRole(long roleId, string displayName, string? description, bool isActive);
        void DeleteRole(long roleId);
        void AssignPermissionsToRole(long roleId, List<PermissionAssignment> permissions);
        List<PermissionDTO> GetRolePermissions(long roleId);
    }

    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _repo;

        public RolePermissionService(IRolePermissionRepository repo)
        {
            _repo = repo;
        }

        public List<RoleDTO> GetRolesByTenantAndProduct(long tenantId, int productId)
        {
            return _repo.GetRolesByTenantAndProduct(tenantId, productId);
        }

        public List<PermissionDTO> GetPermissionsByProduct(int productId)
        {
            return _repo.GetPermissionsByProduct(productId);
        }

        public List<PermissionDTO> GetUserPermissions(int userId)
        {
            return _repo.GetUserPermissions(userId);
        }

        public long CreateRole(long tenantId, int productId, string name, string displayName, string? description)
        {
            // Validate role name (e.g., no special characters, lowercase)
            name = name.ToLower().Replace(" ", "_");
            
            return _repo.CreateRole(tenantId, productId, name, displayName, description);
        }

        public void UpdateRole(long roleId, string displayName, string? description, bool isActive)
        {
            _repo.UpdateRole(roleId, displayName, description, isActive);
        }

        public void DeleteRole(long roleId)
        {
            _repo.DeleteRole(roleId);
        }

        public void AssignPermissionsToRole(long roleId, List<PermissionAssignment> permissions)
        {
            _repo.AssignPermissionsToRole(roleId, permissions);
        }

        public List<PermissionDTO> GetRolePermissions(long roleId)
        {
            return _repo.GetRolePermissions(roleId);
        }
    }
}
