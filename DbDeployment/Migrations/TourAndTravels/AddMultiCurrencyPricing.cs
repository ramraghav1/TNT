using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202604220001)]
    public class AddMultiCurrencyPricing : Migration
    {
        public override void Up()
        {
            // Hotel rooms — existing price_per_night is NPR
            Alter.Table("hotel_rooms")
                .AddColumn("price_per_night_usd").AsDecimal(10, 2).Nullable()
                .AddColumn("price_per_night_inr").AsDecimal(10, 2).Nullable();

            // Vehicles — existing price_per_day & price_per_km are NPR
            Alter.Table("vehicles")
                .AddColumn("price_per_day_usd").AsDecimal(10, 2).Nullable()
                .AddColumn("price_per_day_inr").AsDecimal(10, 2).Nullable()
                .AddColumn("price_per_km_usd").AsDecimal(10, 2).Nullable()
                .AddColumn("price_per_km_inr").AsDecimal(10, 2).Nullable();

            // Guides — existing price_per_day is NPR
            Alter.Table("guides")
                .AddColumn("price_per_day_usd").AsDecimal(10, 2).Nullable()
                .AddColumn("price_per_day_inr").AsDecimal(10, 2).Nullable();

            // Activities — existing price_per_person is NPR
            Alter.Table("activities")
                .AddColumn("price_per_person_usd").AsDecimal(10, 2).Nullable()
                .AddColumn("price_per_person_inr").AsDecimal(10, 2).Nullable();
        }

        public override void Down()
        {
            Delete.Column("price_per_night_usd").FromTable("hotel_rooms");
            Delete.Column("price_per_night_inr").FromTable("hotel_rooms");

            Delete.Column("price_per_day_usd").FromTable("vehicles");
            Delete.Column("price_per_day_inr").FromTable("vehicles");
            Delete.Column("price_per_km_usd").FromTable("vehicles");
            Delete.Column("price_per_km_inr").FromTable("vehicles");

            Delete.Column("price_per_day_usd").FromTable("guides");
            Delete.Column("price_per_day_inr").FromTable("guides");

            Delete.Column("price_per_person_usd").FromTable("activities");
            Delete.Column("price_per_person_inr").FromTable("activities");
        }
    }
}
