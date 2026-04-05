using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608310007)]
    public class CreateRolesAndPermissionsTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // ROLES TABLE (Tenant & Product-specific roles)
            // ============================================================
            Create.Table("roles")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable()
                    .ForeignKey("fk_roles_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("product_id").AsInt32().NotNullable()
                    .ForeignKey("fk_roles_product", "products", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("name").AsString(100).NotNullable()
                .WithColumn("display_name").AsString(200).NotNullable()
                .WithColumn("description").AsString(500).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("is_system_role").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("idx_roles_tenant")
                .OnTable("roles")
                .OnColumn("tenant_id").Ascending();

            Create.Index("idx_roles_product")
                .OnTable("roles")
                .OnColumn("product_id").Ascending();

            Create.Index("uq_role_name_tenant_product")
                .OnTable("roles")
                .OnColumn("tenant_id").Ascending()
                .OnColumn("product_id").Ascending()
                .OnColumn("name").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // PERMISSIONS TABLE (Available menus/features per product)
            // ============================================================
            Create.Table("permissions")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("product_id").AsInt32().NotNullable()
                    .ForeignKey("fk_permissions_product", "products", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("name").AsString(100).NotNullable()           // e.g., "inventory"
                .WithColumn("display_name").AsString(200).NotNullable()   // e.g., "Inventory Management"
                .WithColumn("category").AsString(50).Nullable()            // e.g., "menu", "action"
                .WithColumn("parent_id").AsInt64().Nullable()
                    .ForeignKey("fk_permissions_parent", "permissions", "id").OnDelete(System.Data.Rule.SetNull)
                .WithColumn("route").AsString(255).Nullable()              // e.g., "/inventory"
                .WithColumn("icon").AsString(100).Nullable()               // e.g., "pi pi-box"
                .WithColumn("sort_order").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("idx_permissions_product")
                .OnTable("permissions")
                .OnColumn("product_id").Ascending();

            Create.Index("idx_permissions_parent")
                .OnTable("permissions")
                .OnColumn("parent_id").Ascending();

            // ============================================================
            // ROLE_PERMISSIONS TABLE (Many-to-Many)
            // ============================================================
            Create.Table("role_permissions")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("role_id").AsInt64().NotNullable()
                    .ForeignKey("fk_role_permissions_role", "roles", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("permission_id").AsInt64().NotNullable()
                    .ForeignKey("fk_role_permissions_permission", "permissions", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("can_create").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("can_read").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("can_update").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("can_delete").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("uq_role_permission")
                .OnTable("role_permissions")
                .OnColumn("role_id").Ascending()
                .OnColumn("permission_id").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // USER_ROLES TABLE (Assign roles to users)
            // ============================================================
            Create.Table("user_roles")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable()
                    .ForeignKey("fk_user_roles_user", "userinformation", "userid").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("role_id").AsInt64().NotNullable()
                    .ForeignKey("fk_user_roles_role", "roles", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("assigned_by").AsInt32().Nullable()
                .WithColumn("assigned_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("uq_user_role")
                .OnTable("user_roles")
                .OnColumn("user_id").Ascending()
                .OnColumn("role_id").Ascending()
                .WithOptions().Unique();

            Create.Index("idx_user_roles_user")
                .OnTable("user_roles")
                .OnColumn("user_id").Ascending();

            // ============================================================
            // SEED DEFAULT PERMISSIONS FOR TOURANDTRAVEL
            // ============================================================
            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'inventory', 'Inventory Management', 'menu', '/inventory', 'pi pi-box', 1
                FROM products p WHERE p.name = 'TourAndTravel';
            ");

            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'availability', 'Availability Management', 'menu', '/availability', 'pi pi-calendar', 2
                FROM products p WHERE p.name = 'TourAndTravel';
            ");

            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'itinerary', 'Itinerary Management', 'menu', '/itinerary', 'pi pi-map', 3
                FROM products p WHERE p.name = 'TourAndTravel';
            ");

            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'booking', 'Booking Management', 'menu', '/booking', 'pi pi-ticket', 4
                FROM products p WHERE p.name = 'TourAndTravel';
            ");

            // ============================================================
            // SEED DEFAULT PERMISSIONS FOR CLINIC
            // ============================================================
            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'appointments', 'Appointments', 'menu', '/appointments', 'pi pi-calendar-plus', 1
                FROM products p WHERE p.name = 'Clinic';
            ");

            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'patients', 'Patients', 'menu', '/patients', 'pi pi-users', 2
                FROM products p WHERE p.name = 'Clinic';
            ");

            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'practitioners', 'Practitioners', 'menu', '/practitioners', 'pi pi-user-plus', 3
                FROM products p WHERE p.name = 'Clinic';
            ");

            // ============================================================
            // SEED DEFAULT PERMISSIONS FOR REMITTANCE
            // ============================================================
            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'agents', 'Agents', 'menu', '/agents', 'pi pi-building', 1
                FROM products p WHERE p.name = 'Remittance';
            ");

            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'branches', 'Branches', 'menu', '/branches', 'pi pi-sitemap', 2
                FROM products p WHERE p.name = 'Remittance';
            ");

            Execute.Sql(@"
                INSERT INTO permissions (product_id, name, display_name, category, route, icon, sort_order)
                SELECT p.id, 'transactions', 'Transactions', 'menu', '/transactions', 'pi pi-money-bill', 3
                FROM products p WHERE p.name = 'Remittance';
            ");

            // ============================================================
            // SEED DEFAULT ADMIN ROLE FOR DEMO TENANT
            // ============================================================
            Execute.Sql(@"
                INSERT INTO roles (tenant_id, product_id, name, display_name, description, is_system_role)
                SELECT t.id, tp.product_id, 'admin', 'Administrator', 'Full access to all features', true
                FROM saas_tenants t
                JOIN tenant_products tp ON tp.tenant_id = t.id
                WHERE t.subdomain = 'demo' AND tp.is_active = true;
            ");

            Execute.Sql(@"
                INSERT INTO roles (tenant_id, product_id, name, display_name, description, is_system_role)
                SELECT t.id, tp.product_id, 'manager', 'Manager', 'Manage operations and users', true
                FROM saas_tenants t
                JOIN tenant_products tp ON tp.tenant_id = t.id
                WHERE t.subdomain = 'demo' AND tp.is_active = true;
            ");

            Execute.Sql(@"
                INSERT INTO roles (tenant_id, product_id, name, display_name, description, is_system_role)
                SELECT t.id, tp.product_id, 'user', 'Standard User', 'Basic access', true
                FROM saas_tenants t
                JOIN tenant_products tp ON tp.tenant_id = t.id
                WHERE t.subdomain = 'demo' AND tp.is_active = true;
            ");

            // ============================================================
            // ASSIGN ALL PERMISSIONS TO ADMIN ROLE FOR DEMO TENANT
            // ============================================================
            Execute.Sql(@"
                INSERT INTO role_permissions (role_id, permission_id, can_create, can_read, can_update, can_delete)
                SELECT r.id, p.id, true, true, true, true
                FROM roles r
                JOIN permissions p ON p.product_id = r.product_id
                JOIN saas_tenants t ON t.id = r.tenant_id
                WHERE r.name = 'admin' AND t.subdomain = 'demo';
            ");
        }

        public override void Down()
        {
            Execute.Sql("DROP TABLE IF EXISTS user_roles CASCADE");
            Execute.Sql("DROP TABLE IF EXISTS role_permissions CASCADE");
            Execute.Sql("DROP TABLE IF EXISTS permissions CASCADE");
            Execute.Sql("DROP TABLE IF EXISTS roles CASCADE");
        }
    }
}
