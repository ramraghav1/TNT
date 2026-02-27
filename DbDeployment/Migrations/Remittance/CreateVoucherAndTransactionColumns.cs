using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202602260001)]
    public class CreateVoucherAndTransactionColumns : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1. ADD AGENT/BRANCH COLUMNS TO transaction_detail
            // ============================================================
            Alter.Table("transaction_detail")
                .AddColumn("sender_agent_id").AsInt64().Nullable()
                    .ForeignKey("fk_txn_sender_agent", "agents", "id")
                .AddColumn("sender_branch_id").AsInt64().Nullable()
                    .ForeignKey("fk_txn_sender_branch", "branches", "id")
                .AddColumn("payout_agent_id").AsInt64().Nullable()
                    .ForeignKey("fk_txn_payout_agent", "agents", "id")
                .AddColumn("payout_branch_id").AsInt64().Nullable()
                    .ForeignKey("fk_txn_payout_branch", "branches", "id")
                .AddColumn("status").AsString(30).Nullable().WithDefaultValue("SENT")
                .AddColumn("created_at").AsDateTime().Nullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_txn_sender_agent").OnTable("transaction_detail")
                .OnColumn("sender_agent_id").Ascending();
            Create.Index("ix_txn_sender_branch").OnTable("transaction_detail")
                .OnColumn("sender_branch_id").Ascending();
            Create.Index("ix_txn_payout_agent").OnTable("transaction_detail")
                .OnColumn("payout_agent_id").Ascending();
            Create.Index("ix_txn_payout_branch").OnTable("transaction_detail")
                .OnColumn("payout_branch_id").Ascending();

            // ============================================================
            // 2. ADD current_balance TO branches
            // ============================================================
            Alter.Table("branches")
                .AddColumn("current_balance").AsDecimal(18, 2).NotNullable().WithDefaultValue(0);

            // ============================================================
            // 3. CREATE VOUCHERS TABLE
            //    Unified balance management for agents & branches.
            //    mode: 'CR' (credit / adds balance) or 'DR' (debit / deducts balance)
            //    entity_type: 'AGENT' or 'BRANCH'
            //    reference_type: DEPOSIT, WITHDRAWAL, ADJUSTMENT, SEND, PAYOUT
            // ============================================================
            Create.Table("vouchers")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("voucher_number").AsString(50).NotNullable().Unique()
                .WithColumn("voucher_date").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("entity_type").AsString(20).NotNullable()       // AGENT or BRANCH
                .WithColumn("agent_id").AsInt64().Nullable()
                    .ForeignKey("fk_voucher_agent", "agents", "id")
                .WithColumn("branch_id").AsInt64().Nullable()
                    .ForeignKey("fk_voucher_branch", "branches", "id")
                .WithColumn("agent_account_id").AsInt64().Nullable()
                    .ForeignKey("fk_voucher_agent_account", "agent_accounts", "id")
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("mode").AsString(10).NotNullable()              // CR or DR
                .WithColumn("balance_before").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("balance_after").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("reference_type").AsString(50).NotNullable()    // DEPOSIT, WITHDRAWAL, ADJUSTMENT, SEND, PAYOUT
                .WithColumn("reference_id").AsString(100).Nullable()        // e.g. transaction_id
                .WithColumn("description").AsString(500).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("created_by").AsString(150).Nullable();

            Create.Index("ix_voucher_agent").OnTable("vouchers")
                .OnColumn("agent_id").Ascending();
            Create.Index("ix_voucher_branch").OnTable("vouchers")
                .OnColumn("branch_id").Ascending();
            Create.Index("ix_voucher_account").OnTable("vouchers")
                .OnColumn("agent_account_id").Ascending();
            Create.Index("ix_voucher_date").OnTable("vouchers")
                .OnColumn("voucher_date").Ascending();
            Create.Index("ix_voucher_entity").OnTable("vouchers")
                .OnColumn("entity_type").Ascending()
                .OnColumn("voucher_date").Ascending();
        }

        public override void Down()
        {
            Delete.Table("vouchers");

            Delete.Column("current_balance").FromTable("branches");

            Delete.ForeignKey("fk_txn_sender_agent").OnTable("transaction_detail");
            Delete.ForeignKey("fk_txn_sender_branch").OnTable("transaction_detail");
            Delete.ForeignKey("fk_txn_payout_agent").OnTable("transaction_detail");
            Delete.ForeignKey("fk_txn_payout_branch").OnTable("transaction_detail");
            Delete.Column("sender_agent_id").FromTable("transaction_detail");
            Delete.Column("sender_branch_id").FromTable("transaction_detail");
            Delete.Column("payout_agent_id").FromTable("transaction_detail");
            Delete.Column("payout_branch_id").FromTable("transaction_detail");
            Delete.Column("status").FromTable("transaction_detail");
            Delete.Column("created_at").FromTable("transaction_detail");
        }
    }
}
