using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202602250003)]
    public class CreateAgentAccountTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1. AGENT ACCOUNTS
            //    Stores account information, bank details, currency & balance
            //    One agent can have multiple accounts (one per currency)
            // ============================================================
            Create.Table("agent_accounts")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("agent_id").AsInt64().NotNullable()
                    .ForeignKey("fk_aa_agent", "agents", "id")
                .WithColumn("account_name").AsString(200).NotNullable()
                .WithColumn("account_number").AsString(100).NotNullable()
                .WithColumn("bank_name").AsString(200).Nullable()
                .WithColumn("bank_branch").AsString(200).Nullable()
                .WithColumn("bank_details").AsString(1000).Nullable()   // additional info (SWIFT, IBAN, etc.)
                .WithColumn("currency_code").AsString(10).NotNullable()
                .WithColumn("balance").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("ix_aa_agent_id")
                .OnTable("agent_accounts")
                .OnColumn("agent_id").Ascending();

            Create.Index("ix_aa_agent_currency")
                .OnTable("agent_accounts")
                .OnColumn("agent_id").Ascending()
                .OnColumn("currency_code").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 2. AGENT LEDGER ENTRIES
            //    Every balance movement is recorded here for statement of account.
            //    transaction_type: 'DEBIT' or 'CREDIT'
            //    reference_type:   'SEND', 'PAYOUT', 'ADJUSTMENT', 'DEPOSIT', etc.
            //    reference_id:     FK-free pointer to the source record (e.g. txn id)
            // ============================================================
            Create.Table("agent_ledger_entries")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("agent_account_id").AsInt64().NotNullable()
                    .ForeignKey("fk_ale_agent_account", "agent_accounts", "id")
                .WithColumn("transaction_type").AsString(10).NotNullable()   // DEBIT | CREDIT
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("balance_before").AsDecimal(18, 2).NotNullable()
                .WithColumn("balance_after").AsDecimal(18, 2).NotNullable()
                .WithColumn("reference_type").AsString(50).NotNullable()     // SEND, PAYOUT, ADJUSTMENT, DEPOSIT
                .WithColumn("reference_id").AsString(100).Nullable()         // txn id or other reference
                .WithColumn("description").AsString(500).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("created_by").AsString(150).Nullable();

            Create.Index("ix_ale_agent_account_id")
                .OnTable("agent_ledger_entries")
                .OnColumn("agent_account_id").Ascending();

            Create.Index("ix_ale_created_at")
                .OnTable("agent_ledger_entries")
                .OnColumn("created_at").Ascending();

            Create.Index("ix_ale_reference")
                .OnTable("agent_ledger_entries")
                .OnColumn("reference_type").Ascending()
                .OnColumn("reference_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("agent_ledger_entries");
            Delete.Table("agent_accounts");
        }
    }
}
