using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202602240001)]
    public class CreateFxRateSetupTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // FX_RATE_SETUPS — Main rate configuration table
            //
            // Supports:
            //  • Corridor-level rates (agent_id = NULL)
            //  • Agent-specific rates (agent_id set)
            //  • Multi-currency settlement (settlement_currency differs from sending_currency)
            //  • Customer rate & settlement rate on same record
            // ============================================================
            Create.Table("fx_rate_setups")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()

                // Corridor
                .WithColumn("sending_country_id").AsInt64().NotNullable()
                    .ForeignKey("fk_fxr_sending_country", "countries", "id")
                .WithColumn("receiving_country_id").AsInt64().NotNullable()
                    .ForeignKey("fk_fxr_receiving_country", "countries", "id")

                // Payment type & optional agent
                .WithColumn("payment_type_id").AsInt64().NotNullable()
                    .ForeignKey("fk_fxr_payment_type", "payment_types", "id")
                .WithColumn("agent_id").AsInt64().Nullable()
                    .ForeignKey("fk_fxr_agent", "agents", "id")

                // Currencies
                .WithColumn("sending_currency").AsString(10).NotNullable()       // e.g. JPY
                .WithColumn("receiving_currency").AsString(10).NotNullable()     // e.g. NPR
                .WithColumn("settlement_currency").AsString(10).Nullable()       // e.g. USD — null means same as sending

                // Rates
                .WithColumn("customer_rate").AsDecimal(18, 6).NotNullable()       // Rate shown to customer
                .WithColumn("settlement_rate").AsDecimal(18, 6).Nullable()        // Rate for agent/partner settlement
                .WithColumn("cross_rate").AsDecimal(18, 6).Nullable()             // Multi-currency conversion rate

                // Margin
                .WithColumn("margin_type").AsString(20).Nullable()               // 'Flat' or 'Percentage'
                .WithColumn("margin_value").AsDecimal(18, 6).Nullable()

                // Validity window
                .WithColumn("valid_from").AsDateTime().Nullable()
                .WithColumn("valid_to").AsDateTime().Nullable()

                // Status & audit
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_by").AsString(150).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            // Unique constraint: one active rate per corridor + payment type + agent
            Create.Index("ix_fxr_corridor_unique")
                .OnTable("fx_rate_setups")
                .OnColumn("sending_country_id").Ascending()
                .OnColumn("receiving_country_id").Ascending()
                .OnColumn("payment_type_id").Ascending()
                .OnColumn("agent_id").Ascending();

            Create.Index("ix_fxr_sending_country")
                .OnTable("fx_rate_setups")
                .OnColumn("sending_country_id").Ascending();

            Create.Index("ix_fxr_receiving_country")
                .OnTable("fx_rate_setups")
                .OnColumn("receiving_country_id").Ascending();

            // ============================================================
            // FX_RATE_HISTORY — Audit trail of rate changes
            // ============================================================
            Create.Table("fx_rate_history")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("fx_rate_setup_id").AsInt64().NotNullable()
                    .ForeignKey("fk_fxh_setup", "fx_rate_setups", "id")
                .WithColumn("previous_customer_rate").AsDecimal(18, 6).Nullable()
                .WithColumn("new_customer_rate").AsDecimal(18, 6).NotNullable()
                .WithColumn("previous_settlement_rate").AsDecimal(18, 6).Nullable()
                .WithColumn("new_settlement_rate").AsDecimal(18, 6).Nullable()
                .WithColumn("previous_cross_rate").AsDecimal(18, 6).Nullable()
                .WithColumn("new_cross_rate").AsDecimal(18, 6).Nullable()
                .WithColumn("changed_by").AsString(150).Nullable()
                .WithColumn("changed_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("reason").AsString(500).Nullable();

            Create.Index("ix_fxh_setup_id")
                .OnTable("fx_rate_history")
                .OnColumn("fx_rate_setup_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("fx_rate_history");
            Delete.Table("fx_rate_setups");
        }
    }
}
