using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202604160002)]
    public class AddDescriptionToItineraryDays : Migration
    {
        public override void Up()
        {
            Alter.Table("itinerary_days")
                .AddColumn("description").AsString(2000).Nullable();
        }

        public override void Down()
        {
            Delete.Column("description").FromTable("itinerary_days");
        }
    }
}
