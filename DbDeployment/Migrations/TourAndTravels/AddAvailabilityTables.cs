using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202604020002)]
    public class AddAvailabilityTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // AVAILABILITY
            // ============================================================
            Create.Table("availability")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("inventory_type").AsString(20).NotNullable() // Hotel, Vehicle, Guide, Activity
                .WithColumn("inventory_id").AsInt64().NotNullable()
                .WithColumn("date").AsDate().NotNullable()
                .WithColumn("total_capacity").AsInt32().NotNullable()
                .WithColumn("booked_capacity").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("available_capacity").AsInt32().NotNullable()
                .WithColumn("status").AsString(20).NotNullable().WithDefaultValue("Available") // Available, Limited, Full, Blocked
                .WithColumn("special_price").AsDecimal(10, 2).Nullable()
                .WithColumn("notes").AsString(int.MaxValue).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            // Unique constraint: one availability record per inventory item per date
            Create.Index("uq_availability_inventory_date")
                .OnTable("availability")
                .OnColumn("inventory_type").Ascending()
                .OnColumn("inventory_id").Ascending()
                .OnColumn("date").Ascending()
                .WithOptions().Unique();

            Create.Index("idx_availability_date")
                .OnTable("availability")
                .OnColumn("date").Ascending();

            Create.Index("idx_availability_type")
                .OnTable("availability")
                .OnColumn("inventory_type").Ascending();

            Create.Index("idx_availability_inventory")
                .OnTable("availability")
                .OnColumn("inventory_id").Ascending();

            Create.Index("idx_availability_status")
                .OnTable("availability")
                .OnColumn("status").Ascending();

            // ============================================================
            // AVAILABILITY BLOCKS
            // ============================================================
            Create.Table("availability_blocks")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("inventory_type").AsString(20).NotNullable()
                .WithColumn("inventory_id").AsInt64().NotNullable()
                .WithColumn("start_date").AsDate().NotNullable()
                .WithColumn("end_date").AsDate().NotNullable()
                .WithColumn("reason").AsString(100).NotNullable()
                .WithColumn("notes").AsString(int.MaxValue).Nullable()
                .WithColumn("created_by").AsInt32().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("idx_blocks_dates")
                .OnTable("availability_blocks")
                .OnColumn("start_date").Ascending()
                .OnColumn("end_date").Ascending();

            Create.Index("idx_blocks_inventory")
                .OnTable("availability_blocks")
                .OnColumn("inventory_type").Ascending()
                .OnColumn("inventory_id").Ascending();

            // ============================================================
            // PACKAGE DEPARTURES
            // ============================================================
            Create.Table("package_departures")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_id").AsInt64().NotNullable()
                    .ForeignKey("fk_package_itinerary", "itineraries", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("departure_date").AsDate().NotNullable()
                .WithColumn("min_group_size").AsInt32().NotNullable()
                .WithColumn("max_group_size").AsInt32().NotNullable()
                .WithColumn("current_bookings").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("available_seats").AsInt32().NotNullable()
                .WithColumn("status").AsString(30).NotNullable().WithDefaultValue("OpenForBooking")
                .WithColumn("package_price").AsDecimal(10, 2).NotNullable()
                .WithColumn("is_guaranteed_departure").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("notes").AsString(int.MaxValue).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            // Unique constraint: one departure per itinerary per date
            Create.Index("uq_package_departure")
                .OnTable("package_departures")
                .OnColumn("itinerary_id").Ascending()
                .OnColumn("departure_date").Ascending()
                .WithOptions().Unique();

            Create.Index("idx_package_departure_date")
                .OnTable("package_departures")
                .OnColumn("departure_date").Ascending();

            Create.Index("idx_package_itinerary")
                .OnTable("package_departures")
                .OnColumn("itinerary_id").Ascending();

            Create.Index("idx_package_status")
                .OnTable("package_departures")
                .OnColumn("status").Ascending();

            // ============================================================
            // BOOKING INVENTORY LINKS
            // ============================================================
            Create.Table("booking_inventory")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("booking_instance_id").AsInt64().NotNullable()
                    .ForeignKey("fk_booking_inventory_instance", "itinerary_instances", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("inventory_type").AsString(20).NotNullable()
                .WithColumn("inventory_id").AsInt64().NotNullable()
                .WithColumn("start_date").AsDate().NotNullable()
                .WithColumn("end_date").AsDate().NotNullable()
                .WithColumn("quantity").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("price").AsDecimal(10, 2).NotNullable()
                .WithColumn("notes").AsString(int.MaxValue).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("idx_booking_inventory_instance")
                .OnTable("booking_inventory")
                .OnColumn("booking_instance_id").Ascending();

            Create.Index("idx_booking_inventory_type")
                .OnTable("booking_inventory")
                .OnColumn("inventory_type").Ascending()
                .OnColumn("inventory_id").Ascending();

            Create.Index("idx_booking_inventory_dates")
                .OnTable("booking_inventory")
                .OnColumn("start_date").Ascending()
                .OnColumn("end_date").Ascending();
        }

        public override void Down()
        {
            Delete.Table("booking_inventory");
            Delete.Table("package_departures");
            Delete.Table("availability_blocks");
            Delete.Table("availability");
        }
    }
}
