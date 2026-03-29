using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202603290001)]
    public class AddPriceToItineraryDayCosts : Migration
    {
        public override void Up()
        {
            Alter.Table("itinerary_day_costs")
                .AddColumn("price").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .AddColumn("currency").AsString(20).NotNullable().WithDefaultValue("NPR");
        }

        public override void Down()
        {
            Delete.Column("price").FromTable("itinerary_day_costs");
            Delete.Column("currency").FromTable("itinerary_day_costs");
        }
    }
}
