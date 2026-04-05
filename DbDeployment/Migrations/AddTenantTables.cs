using FluentMigrator;
using System;

namespace DbDeployment.Migrations
{
    [Migration(20260402000001)]
    public class AddTenantTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // PRODUCTS TABLE
            // ============================================================
            Create.Table("products")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("name").AsString(50).NotNullable().Unique()
                .WithColumn("display_name").AsString(100).NotNullable()
                .WithColumn("description").AsString(500).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true);

            // Insert default products
            Insert.IntoTable("products").Row(new 
            { 
                name = "TourAndTravel", 
                display_name = "Tour & Travel Management",
                description = "Complete tour and travel booking system with itinerary management"
            });
            Insert.IntoTable("products").Row(new 
            { 
                name = "Clinic", 
                display_name = "Clinic Management",
                description = "Healthcare clinic management system"
            });
            Insert.IntoTable("products").Row(new 
            { 
                name = "Remittance", 
                display_name = "Remittance Services",
                description = "Money transfer and remittance management"
            });

            // ============================================================
            // SAAS TENANTS TABLE (renamed to avoid conflict with Clinic module's tenants table)
            // ============================================================
            Create.Table("saas_tenants")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("subdomain").AsString(100).NotNullable().Unique()
                .WithColumn("logo_url").AsString(500).Nullable()
                .WithColumn("contact_email").AsString(255).NotNullable()
                .WithColumn("contact_phone").AsString(50).Nullable()
                .WithColumn("status").AsString(20).NotNullable().WithDefaultValue("Active")
                .WithColumn("settings_json").AsCustom("JSONB").NotNullable().WithDefaultValue("{}")
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("idx_saas_tenants_subdomain")
                .OnTable("saas_tenants")
                .OnColumn("subdomain").Ascending();

            Create.Index("idx_saas_tenants_status")
                .OnTable("saas_tenants")
                .OnColumn("status").Ascending();

            // ============================================================
            // TENANT_PRODUCTS TABLE (Mapping)
            // ============================================================
            Create.Table("tenant_products")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable()
                    .ForeignKey("fk_tenant_products_tenant", "saas_tenants", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("product_id").AsInt32().NotNullable()
                    .ForeignKey("fk_tenant_products_product", "products", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("activated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("deactivated_at").AsDateTime().Nullable()
                .WithColumn("subscription_tier").AsString(50).Nullable();

            // Unique constraint: one product per tenant
            Create.Index("uq_tenant_product")
                .OnTable("tenant_products")
                .OnColumn("tenant_id").Ascending()
                .OnColumn("product_id").Ascending()
                .WithOptions().Unique();

            Create.Index("idx_tenant_products_tenant")
                .OnTable("tenant_products")
                .OnColumn("tenant_id").Ascending();

            Create.Index("idx_tenant_products_active")
                .OnTable("tenant_products")
                .OnColumn("is_active").Ascending();

            // ============================================================
            // CREATE DEFAULT DEMO TENANT
            // ============================================================
            Execute.Sql(@"
                INSERT INTO saas_tenants (name, subdomain, contact_email, status, settings_json)
                VALUES ('Demo Company', 'demo', 'demo@example.com', 'Active', 
                    '{""maxUsers"": 50, ""maxBookingsPerMonth"": 500, ""isTrialAccount"": true, ""timeZone"": ""UTC"", ""currency"": ""USD""}');
            ");

            Execute.Sql(@"
                INSERT INTO tenant_products (tenant_id, product_id, is_active)
                SELECT t.id, p.id, true
                FROM saas_tenants t
                CROSS JOIN products p
                WHERE t.subdomain = 'demo' AND p.name = 'TourAndTravel';
            ");
        }

        public override void Down()
        {
            // Drop tables with CASCADE to remove any remaining foreign key constraints
            Execute.Sql("DROP TABLE IF EXISTS tenant_products CASCADE");
            Execute.Sql("DROP TABLE IF EXISTS saas_tenants CASCADE");
            Execute.Sql("DROP TABLE IF EXISTS products CASCADE");
        }
    }
}
