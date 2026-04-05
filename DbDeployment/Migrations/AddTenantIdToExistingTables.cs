using FluentMigrator;

namespace DbDeployment.Migrations
{
    /// <summary>
    /// Adds tenant_id column to all business tables for multi-tenancy support
    /// Run this AFTER creating the tenants table (Migration 20260402000001)
    /// </summary>
    [Migration(20260402000002)]
    public class AddTenantIdToExistingTables : Migration
    {
        public override void Up()
        {
            // Get the default tenant ID (demo tenant)
            var defaultTenantId = 1; // Adjust based on your seed data

            // ============================================================
            // TOUR & TRAVEL TABLES
            // ============================================================

            // Itineraries
            if (!Schema.Table("itineraries").Column("tenant_id").Exists())
            {
                Alter.Table("itineraries")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_itineraries_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_itineraries_tenant")
                    .OnTable("itineraries")
                    .OnColumn("tenant_id").Ascending();
            }

            // Itinerary Instances (Bookings)
            if (!Schema.Table("itinerary_instances").Column("tenant_id").Exists())
            {
                Alter.Table("itinerary_instances")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_itinerary_instances_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_itinerary_instances_tenant")
                    .OnTable("itinerary_instances")
                    .OnColumn("tenant_id").Ascending();
            }

            // Hotels
            if (!Schema.Table("hotels").Column("tenant_id").Exists())
            {
                Alter.Table("hotels")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_hotels_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_hotels_tenant")
                    .OnTable("hotels")
                    .OnColumn("tenant_id").Ascending();
            }

            // Vehicles
            if (!Schema.Table("vehicles").Column("tenant_id").Exists())
            {
                Alter.Table("vehicles")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_vehicles_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_vehicles_tenant")
                    .OnTable("vehicles")
                    .OnColumn("tenant_id").Ascending();
            }

            // Guides
            if (!Schema.Table("guides").Column("tenant_id").Exists())
            {
                Alter.Table("guides")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_guides_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_guides_tenant")
                    .OnTable("guides")
                    .OnColumn("tenant_id").Ascending();
            }

            // Activities
            if (!Schema.Table("activities").Column("tenant_id").Exists())
            {
                Alter.Table("activities")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_activities_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_activities_tenant")
                    .OnTable("activities")
                    .OnColumn("tenant_id").Ascending();
            }

            // Availability
            if (!Schema.Table("availability").Column("tenant_id").Exists())
            {
                Alter.Table("availability")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_availability_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_availability_tenant")
                    .OnTable("availability")
                    .OnColumn("tenant_id").Ascending();
            }

            // Package Departures
            if (!Schema.Table("package_departures").Column("tenant_id").Exists())
            {
                Alter.Table("package_departures")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_package_departures_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_package_departures_tenant")
                    .OnTable("package_departures")
                    .OnColumn("tenant_id").Ascending();
            }

            // ============================================================
            // USER & ORGANIZATION TABLES
            // ============================================================

            // Users - if you want per-tenant users
            if (Schema.Table("users").Exists() && !Schema.Table("users").Column("tenant_id").Exists())
            {
                Alter.Table("users")
                    .AddColumn("tenant_id").AsInt64().Nullable() // Nullable for super-admins
                    .ForeignKey("fk_users_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.SetNull);

                Create.Index("idx_users_tenant")
                    .OnTable("users")
                    .OnColumn("tenant_id").Ascending();
            }

            // Organizations
            if (Schema.Table("organizations").Exists() && !Schema.Table("organizations").Column("tenant_id").Exists())
            {
                Alter.Table("organizations")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_organizations_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_organizations_tenant")
                    .OnTable("organizations")
                    .OnColumn("tenant_id").Ascending();
            }

            // ============================================================
            // REMITTANCE TABLES (if applicable)
            // ============================================================

            if (Schema.Table("agents").Exists() && !Schema.Table("agents").Column("tenant_id").Exists())
            {
                Alter.Table("agents")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_agents_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_agents_tenant")
                    .OnTable("agents")
                    .OnColumn("tenant_id").Ascending();
            }

            if (Schema.Table("branches").Exists() && !Schema.Table("branches").Column("tenant_id").Exists())
            {
                Alter.Table("branches")
                    .AddColumn("tenant_id").AsInt64().NotNullable().WithDefaultValue(defaultTenantId)
                    .ForeignKey("fk_branches_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade);

                Create.Index("idx_branches_tenant")
                    .OnTable("branches")
                    .OnColumn("tenant_id").Ascending();
            }

            // Add tenant_id to other relevant tables as needed...
        }

        public override void Down()
        {
            // Remove tenant_id columns in reverse order

            if (Schema.Table("branches").Column("tenant_id").Exists())
            {
                Delete.ForeignKey("fk_branches_tenant").OnTable("branches");
                Delete.Index("idx_branches_tenant").OnTable("branches");
                Delete.Column("tenant_id").FromTable("branches");
            }

            if (Schema.Table("agents").Column("tenant_id").Exists())
            {
                Delete.ForeignKey("fk_agents_tenant").OnTable("agents");
                Delete.Index("idx_agents_tenant").OnTable("agents");
                Delete.Column("tenant_id").FromTable("agents");
            }

            if (Schema.Table("organizations").Column("tenant_id").Exists())
            {
                Delete.ForeignKey("fk_organizations_tenant").OnTable("organizations");
                Delete.Index("idx_organizations_tenant").OnTable("organizations");
                Delete.Column("tenant_id").FromTable("organizations");
            }

            Delete.ForeignKey("fk_users_tenant").OnTable("users");
            Delete.Index("idx_users_tenant").OnTable("users");
            Delete.Column("tenant_id").FromTable("users");

            Delete.ForeignKey("fk_package_departures_tenant").OnTable("package_departures");
            Delete.Index("idx_package_departures_tenant").OnTable("package_departures");
            Delete.Column("tenant_id").FromTable("package_departures");

            Delete.ForeignKey("fk_availability_tenant").OnTable("availability");
            Delete.Index("idx_availability_tenant").OnTable("availability");
            Delete.Column("tenant_id").FromTable("availability");

            Delete.ForeignKey("fk_activities_tenant").OnTable("activities");
            Delete.Index("idx_activities_tenant").OnTable("activities");
            Delete.Column("tenant_id").FromTable("activities");

            Delete.ForeignKey("fk_guides_tenant").OnTable("guides");
            Delete.Index("idx_guides_tenant").OnTable("guides");
            Delete.Column("tenant_id").FromTable("guides");

            Delete.ForeignKey("fk_vehicles_tenant").OnTable("vehicles");
            Delete.Index("idx_vehicles_tenant").OnTable("vehicles");
            Delete.Column("tenant_id").FromTable("vehicles");

            Delete.ForeignKey("fk_hotels_tenant").OnTable("hotels");
            Delete.Index("idx_hotels_tenant").OnTable("hotels");
            Delete.Column("tenant_id").FromTable("hotels");

            Delete.ForeignKey("fk_itinerary_instances_tenant").OnTable("itinerary_instances");
            Delete.Index("idx_itinerary_instances_tenant").OnTable("itinerary_instances");
            Delete.Column("tenant_id").FromTable("itinerary_instances");

            Delete.ForeignKey("fk_itineraries_tenant").OnTable("itineraries");
            Delete.Index("idx_itineraries_tenant").OnTable("itineraries");
            Delete.Column("tenant_id").FromTable("itineraries");
        }
    }
}
