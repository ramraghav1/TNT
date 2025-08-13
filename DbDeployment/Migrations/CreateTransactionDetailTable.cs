using FluentMigrator;

[Migration(20250808002)]
public class CreateTransactionDetailTable : Migration
{
    public override void Up()
    {
        Create.Table("transaction_detail")
            .WithColumn("transaction_id").AsInt32().PrimaryKey().Identity()

            // Transaction
            .WithColumn("payment_type").AsString().Nullable()
            .WithColumn("payout_location").AsString().Nullable()
            .WithColumn("collected_amount").AsDecimal().Nullable()
            .WithColumn("service_fee").AsDecimal().Nullable()
            .WithColumn("transfer_amount").AsDecimal().Nullable()

            // Sender
            .WithColumn("sender_name").AsString().Nullable()
            .WithColumn("sender_address").AsString().Nullable()
            .WithColumn("sender_mobile").AsString().Nullable()

            // Receiver
            .WithColumn("receiver_name").AsString().Nullable()
            .WithColumn("receiver_address").AsString().Nullable()
            .WithColumn("receiver_mobile").AsString().Nullable();
    }

    public override void Down()
    {
        Delete.Table("transaction_detail");
    }
}
