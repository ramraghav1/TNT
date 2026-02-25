using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202602250005)]
    public class CreateDomesticServiceChargeTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 0. Insert Bank Transfer payment type
            // ============================================================
            Execute.Sql(@"
                INSERT INTO payment_types (name, description, is_active, created_at)
                SELECT 'Bank Transfer', 'Account Deposit', true, NOW()
                WHERE NOT EXISTS (SELECT 1 FROM payment_types WHERE name = 'Bank Transfer');
            ");

            // ============================================================
            // 1. CONFIGURATION TYPES (Master)
            //    e.g. 'Agent Category', 'Service Type', etc.
            // ============================================================
            Create.Table("configuration_types")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_ct_name")
                .OnTable("configuration_types")
                .OnColumn("name").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 2. CONFIGURATIONS (Detail)
            //    e.g. ConfigurationType='Agent Category' → 'A', 'B', 'C'
            // ============================================================
            Create.Table("configurations")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("configuration_type_id").AsInt64().NotNullable()
                    .ForeignKey("fk_cfg_type", "configuration_types", "id")
                .WithColumn("code").AsString(50).NotNullable()
                .WithColumn("display_name").AsString(200).NotNullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_cfg_type_code")
                .OnTable("configurations")
                .OnColumn("configuration_type_id").Ascending()
                .OnColumn("code").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 3. ADD category_id TO AGENTS
            // ============================================================
            Alter.Table("agents")
                .AddColumn("category_id").AsInt64().Nullable()
                    .ForeignKey("fk_agents_category", "configurations", "id");

            // ============================================================
            // 4. DOMESTIC SERVICE CHARGE SETUPS
            //    Like service_charge_setups but with from_category_id / to_category_id
            // ============================================================
            Create.Table("domestic_service_charge_setups")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("from_category_id").AsInt64().NotNullable()
                    .ForeignKey("fk_dscs_from_category", "configurations", "id")
                .WithColumn("to_category_id").AsInt64().NotNullable()
                    .ForeignKey("fk_dscs_to_category", "configurations", "id")
                .WithColumn("payment_type_id").AsInt64().NotNullable()
                    .ForeignKey("fk_dscs_payment_type", "payment_types", "id")
                .WithColumn("agent_id").AsInt64().Nullable()
                    .ForeignKey("fk_dscs_agent", "agents", "id")
                .WithColumn("charge_mode").AsString(20).NotNullable()   // 'Flat' or 'Range'
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_by").AsString(150).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("ix_dscs_categories")
                .OnTable("domestic_service_charge_setups")
                .OnColumn("from_category_id").Ascending()
                .OnColumn("to_category_id").Ascending()
                .OnColumn("payment_type_id").Ascending();

            // ============================================================
            // 5. DOMESTIC SERVICE CHARGE SLABS
            //    Same structure as service_charge_slabs, FK to domestic setups
            // ============================================================
            Create.Table("domestic_service_charge_slabs")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("setup_id").AsInt64().NotNullable()
                    .ForeignKey("fk_dslab_setup", "domestic_service_charge_setups", "id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("min_amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("max_amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("charge_type").AsString(20).NotNullable()   // 'Flat' or 'Percentage'
                .WithColumn("charge_value").AsDecimal(18, 4).NotNullable()
                .WithColumn("currency").AsString(10).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_dslabs_setup_id")
                .OnTable("domestic_service_charge_slabs")
                .OnColumn("setup_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("domestic_service_charge_slabs");
            Delete.Table("domestic_service_charge_setups");
            Delete.ForeignKey("fk_agents_category").OnTable("agents");
            Delete.Column("category_id").FromTable("agents");
            Delete.Table("configurations");
            Delete.Table("configuration_types");
        }
    }
}
