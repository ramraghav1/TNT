using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using static Repository.DataModels.UserManagementDTO;

namespace Repository.Repositories
{
    public interface IRolePermissionRepository
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

    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly IDbConnection _dbConnection;

        public RolePermissionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public List<RoleDTO> GetRolesByTenantAndProduct(long tenantId, int productId)
        {
            string sql = @"
                SELECT id AS Id, tenant_id AS TenantId, product_id AS ProductId, name AS Name, 
                       display_name AS DisplayName, description AS Description, 
                       is_active AS IsActive, is_system_role AS IsSystemRole
                FROM roles
                WHERE tenant_id = @TenantId AND product_id = @ProductId AND is_active = true
                ORDER BY is_system_role DESC, display_name";

            return _dbConnection.Query<RoleDTO>(sql, new { TenantId = tenantId, ProductId = productId }).ToList();
        }

        public List<PermissionDTO> GetPermissionsByProduct(int productId)
        {
            string sql = @"
                SELECT id AS Id, product_id AS ProductId, name AS Name, display_name AS DisplayName,
                       category AS Category, parent_id AS ParentId, route AS Route, icon AS Icon,
                       sort_order AS SortOrder
                FROM permissions
                WHERE product_id = @ProductId AND is_active = true
                ORDER BY sort_order, display_name";

            return _dbConnection.Query<PermissionDTO>(sql, new { ProductId = productId }).ToList();
        }

        public List<PermissionDTO> GetUserPermissions(int userId)
        {
            string sql = @"
                SELECT DISTINCT
                    p.id AS Id,
                    p.product_id AS ProductId,
                    p.name AS Name,
                    p.display_name AS DisplayName,
                    p.category AS Category,
                    p.parent_id AS ParentId,
                    p.route AS Route,
                    p.icon AS Icon,
                    p.sort_order AS SortOrder,
                    MAX(CAST(rp.can_create AS INT)) = 1 AS CanCreate,
                    MAX(CAST(rp.can_read AS INT)) = 1 AS CanRead,
                    MAX(CAST(rp.can_update AS INT)) = 1 AS CanUpdate,
                    MAX(CAST(rp.can_delete AS INT)) = 1 AS CanDelete
                FROM user_roles ur
                JOIN role_permissions rp ON rp.role_id = ur.role_id
                JOIN permissions p ON p.id = rp.permission_id
                WHERE ur.user_id = @UserId AND p.is_active = true
                GROUP BY p.id, p.product_id, p.name, p.display_name, p.category, p.parent_id, p.route, p.icon, p.sort_order
                ORDER BY p.sort_order, p.display_name";

            return _dbConnection.Query<PermissionDTO>(sql, new { UserId = userId }).ToList();
        }

        public long CreateRole(long tenantId, int productId, string name, string displayName, string? description)
        {
            string sql = @"
                INSERT INTO roles (tenant_id, product_id, name, display_name, description, is_system_role)
                VALUES (@TenantId, @ProductId, @Name, @DisplayName, @Description, false)
                RETURNING id";

            return _dbConnection.QuerySingle<long>(sql, new 
            { 
                TenantId = tenantId,
                ProductId = productId, 
                Name = name, 
                DisplayName = displayName, 
                Description = description 
            });
        }

        public void UpdateRole(long roleId, string displayName, string? description, bool isActive)
        {
            string sql = @"
                UPDATE roles
                SET display_name = @DisplayName,
                    description = @Description,
                    is_active = @IsActive,
                    updated_at = NOW()
                WHERE id = @RoleId AND is_system_role = false";

            _dbConnection.Execute(sql, new 
            { 
                RoleId = roleId, 
                DisplayName = displayName, 
                Description = description, 
                IsActive = isActive 
            });
        }

        public void DeleteRole(long roleId)
        {
            // Only allow deletion of non-system roles
            string sql = "DELETE FROM roles WHERE id = @RoleId AND is_system_role = false";
            _dbConnection.Execute(sql, new { RoleId = roleId });
        }

        public void AssignPermissionsToRole(long roleId, List<PermissionAssignment> permissions)
        {
            // Delete existing permissions
            _dbConnection.Execute("DELETE FROM role_permissions WHERE role_id = @RoleId", new { RoleId = roleId });

            if (permissions != null && permissions.Any())
            {
                string sql = @"
                    INSERT INTO role_permissions (role_id, permission_id, can_create, can_read, can_update, can_delete)
                    VALUES (@RoleId, @PermissionId, @CanCreate, @CanRead, @CanUpdate, @CanDelete)";

                foreach (var perm in permissions)
                {
                    _dbConnection.Execute(sql, new 
                    { 
                        RoleId = roleId, 
                        PermissionId = perm.PermissionId,
                        CanCreate = perm.CanCreate,
                        CanRead = perm.CanRead,
                        CanUpdate = perm.CanUpdate,
                        CanDelete = perm.CanDelete
                    });
                }
            }
        }

        public List<PermissionDTO> GetRolePermissions(long roleId)
        {
            string sql = @"
                SELECT 
                    p.id AS Id,
                    p.product_id AS ProductId,
                    p.name AS Name,
                    p.display_name AS DisplayName,
                    p.category AS Category,
                    p.parent_id AS ParentId,
                    p.route AS Route,
                    p.icon AS Icon,
                    p.sort_order AS SortOrder,
                    rp.can_create AS CanCreate,
                    rp.can_read AS CanRead,
                    rp.can_update AS CanUpdate,
                    rp.can_delete AS CanDelete
                FROM role_permissions rp
                JOIN permissions p ON p.id = rp.permission_id
                WHERE rp.role_id = @RoleId
                ORDER BY p.sort_order, p.display_name";

            return _dbConnection.Query<PermissionDTO>(sql, new { RoleId = roleId }).ToList();
        }
    }
}
