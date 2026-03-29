using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202603030001)]
    public class AddCommissionColumnsToDomesticSlabs : Migration
    {
        public override void Up()
        {
            Alter.Table("domestic_service_charge_slabs")
                .AddColumn("send_commission").AsDecimal(18, 4).NotNullable().WithDefaultValue(0)
                .AddColumn("payout_commission").AsDecimal(18, 4).NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("send_commission").FromTable("domestic_service_charge_slabs");
            Delete.Column("payout_commission").FromTable("domestic_service_charge_slabs");
        }
    }
}
