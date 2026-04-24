using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    /// <summary>
    /// Adds hotel_id and guide_id columns to itinerary_days table
    /// so each day plan can optionally reference a hotel and/or guide from inventory.
    /// </summary>
    [Migration(202604230001)]
    public class AddHotelGuideToItineraryDays : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("itinerary_days").Column("hotel_id").Exists())
            {
                Alter.Table("itinerary_days")
                    .AddColumn("hotel_id").AsInt64().Nullable()
                    .AddColumn("guide_id").AsInt64().Nullable();

                // Add foreign keys
                Create.ForeignKey("fk_itinerary_days_hotel")
                    .FromTable("itinerary_days").ForeignColumn("hotel_id")
                    .ToTable("hotels").PrimaryColumn("id");

                Create.ForeignKey("fk_itinerary_days_guide")
                    .FromTable("itinerary_days").ForeignColumn("guide_id")
                    .ToTable("guides").PrimaryColumn("id");

                // Add indexes for faster lookups
                Create.Index("ix_itinerary_days_hotel_id")
                    .OnTable("itinerary_days")
                    .OnColumn("hotel_id").Ascending();

                Create.Index("ix_itinerary_days_guide_id")
                    .OnTable("itinerary_days")
                    .OnColumn("guide_id").Ascending();
            }
        }

        public override void Down()
        {
            Delete.Index("ix_itinerary_days_guide_id").OnTable("itinerary_days");
            Delete.Index("ix_itinerary_days_hotel_id").OnTable("itinerary_days");
            Delete.ForeignKey("fk_itinerary_days_guide").OnTable("itinerary_days");
            Delete.ForeignKey("fk_itinerary_days_hotel").OnTable("itinerary_days");
            Delete.Column("guide_id").FromTable("itinerary_days");
            Delete.Column("hotel_id").FromTable("itinerary_days");
        }
    }
}
