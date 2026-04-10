using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    /// <summary>
    /// Adds departure management columns:
    ///  - itinerary_instances.departure_id   → FK to package_departures
    ///  - itinerary_instances.total_person   → head-count for this booking
    ///  - package_departures.guide_id        → FK to guides
    ///  - package_departures.vehicle_id      → FK to vehicles
    ///  - package_departures.end_date        → end date of departure trip
    /// </summary>
    [Migration(202604100001)]
    public class AddDepartureManagementColumns : Migration
    {
        public override void Up()
        {
            // ── itinerary_instances ──────────────────────────────────
            Alter.Table("itinerary_instances")
                .AddColumn("departure_id").AsInt64().Nullable()
                .AddColumn("total_person").AsInt32().NotNullable().WithDefaultValue(1);

            Create.ForeignKey("fk_instances_departure")
                .FromTable("itinerary_instances").ForeignColumn("departure_id")
                .ToTable("package_departures").PrimaryColumn("id");

            Create.Index("ix_instances_departure_id")
                .OnTable("itinerary_instances")
                .OnColumn("departure_id").Ascending();

            // ── package_departures ───────────────────────────────────
            Alter.Table("package_departures")
                .AddColumn("guide_id").AsInt64().Nullable()
                .AddColumn("vehicle_id").AsInt64().Nullable()
                .AddColumn("end_date").AsDate().Nullable();
        }

        public override void Down()
        {
            Delete.ForeignKey("fk_instances_departure").OnTable("itinerary_instances");
            Delete.Index("ix_instances_departure_id").OnTable("itinerary_instances");
            Delete.Column("departure_id").FromTable("itinerary_instances");
            Delete.Column("total_person").FromTable("itinerary_instances");
            Delete.Column("guide_id").FromTable("package_departures");
            Delete.Column("vehicle_id").FromTable("package_departures");
            Delete.Column("end_date").FromTable("package_departures");
        }
    }
}
