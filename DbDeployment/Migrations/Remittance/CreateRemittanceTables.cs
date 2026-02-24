using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(2026022301)]
    public class CreateRemittanceTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1. COUNTRIES (Master)
            // ============================================================
            Create.Table("countries")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("iso2_code").AsString(2).NotNullable()
                .WithColumn("iso3_code").AsString(3).NotNullable()
                .WithColumn("phone_code").AsString(10).Nullable()
                .WithColumn("currency_code").AsString(10).NotNullable()
                .WithColumn("currency_name").AsString(100).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_countries_iso2_code")
                .OnTable("countries")
                .OnColumn("iso2_code").Ascending()
                .WithOptions().Unique();

            Create.Index("ix_countries_iso3_code")
                .OnTable("countries")
                .OnColumn("iso3_code").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 2. PAYMENT TYPES (Master)
            // ============================================================
            Create.Table("payment_types")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(100).NotNullable()
                .WithColumn("description").AsString(500).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_payment_types_name")
                .OnTable("payment_types")
                .OnColumn("name").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 3. AGENTS
            // ============================================================
            Create.Table("agents")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("country_id").AsInt64().NotNullable()
                .ForeignKey("fk_agents_country", "countries", "id")
                .WithColumn("contact_person").AsString(200).Nullable()
                .WithColumn("contact_email").AsString(200).Nullable()
                .WithColumn("contact_phone").AsString(50).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_agents_country_id")
                .OnTable("agents")
                .OnColumn("country_id").Ascending();

            // ============================================================
            // 4. SERVICE CHARGE SETUPS (Header)
            //    Links: sending country, receiving country, payment type, agent
            //    charge_mode: 'Flat' = one slab for all amounts
            //                 'Range' = multiple slabs by amount range
            // ============================================================
            Create.Table("service_charge_setups")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("sending_country_id").AsInt64().NotNullable()
                .ForeignKey("fk_scs_sending_country", "countries", "id")
                .WithColumn("receiving_country_id").AsInt64().NotNullable()
                .ForeignKey("fk_scs_receiving_country", "countries", "id")
                .WithColumn("payment_type_id").AsInt64().NotNullable()
                .ForeignKey("fk_scs_payment_type", "payment_types", "id")
                .WithColumn("agent_id").AsInt64().Nullable()
                .ForeignKey("fk_scs_agent", "agents", "id")
                .WithColumn("charge_mode").AsString(20).NotNullable()   // 'Flat' or 'Range'
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_by").AsString(150).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("ix_scs_corridor")
                .OnTable("service_charge_setups")
                .OnColumn("sending_country_id").Ascending()
                .OnColumn("receiving_country_id").Ascending()
                .OnColumn("payment_type_id").Ascending();

            // ============================================================
            // 5. SERVICE CHARGE SLABS (Detail rows under each setup)
            //    charge_type: 'Flat' = fixed amount, 'Percentage' = % of send amount
            // ============================================================
            Create.Table("service_charge_slabs")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("setup_id").AsInt64().NotNullable()
                .ForeignKey("fk_slab_setup", "service_charge_setups", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("min_amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("max_amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("charge_type").AsString(20).NotNullable()   // 'Flat' or 'Percentage'
                .WithColumn("charge_value").AsDecimal(18, 4).NotNullable()
                .WithColumn("currency").AsString(10).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_slabs_setup_id")
                .OnTable("service_charge_slabs")
                .OnColumn("setup_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("service_charge_slabs");
            Delete.Table("service_charge_setups");
            Delete.Table("agents");
            Delete.Table("payment_types");
            Delete.Table("countries");
        }
    }
}
