using System;
using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202604150001)]
    public class AddItineraryPricingMode : Migration
    {
        public override void Up()
        {
            // Add pricing_mode to itineraries ('OVERALL', 'DAILY', 'DAILY_ACTIVITY')
            Alter.Table("itineraries")
                .AddColumn("pricing_mode").AsString(20).NotNullable().WithDefaultValue("DAILY_ACTIVITY")
                .AddColumn("overall_price").AsDecimal(18, 2).Nullable();

            // Add daily_cost to itinerary_days (used in DAILY pricing mode)
            Alter.Table("itinerary_days")
                .AddColumn("daily_cost").AsDecimal(18, 2).Nullable();
        }

        public override void Down()
        {
            Delete.Column("pricing_mode").FromTable("itineraries");
            Delete.Column("overall_price").FromTable("itineraries");
            Delete.Column("daily_cost").FromTable("itinerary_days");
        }
    }
}
