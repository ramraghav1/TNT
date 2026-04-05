using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202604020001)]
    public class AddInventoryTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // HOTELS
            // ============================================================
            Create.Table("hotels")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("location").AsString(100).NotNullable()
                .WithColumn("address").AsString(500).NotNullable()
                .WithColumn("contact_person").AsString(100).Nullable()
                .WithColumn("phone").AsString(20).Nullable()
                .WithColumn("email").AsString(100).Nullable()
                .WithColumn("star_rating").AsInt32().Nullable()
                .WithColumn("category").AsString(50).NotNullable().WithDefaultValue("Standard")
                .WithColumn("description").AsString(int.MaxValue).Nullable()
                .WithColumn("amenities").AsString(int.MaxValue).Nullable() // JSON array
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable()
                .WithColumn("created_by").AsInt32().NotNullable();

            Create.Index("idx_hotels_location")
                .OnTable("hotels")
                .OnColumn("location").Ascending();

            Create.Index("idx_hotels_category")
                .OnTable("hotels")
                .OnColumn("category").Ascending();

            Create.Index("idx_hotels_active")
                .OnTable("hotels")
                .OnColumn("is_active").Ascending();

            // Hotel Rooms
            Create.Table("hotel_rooms")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("hotel_id").AsInt64().NotNullable()
                    .ForeignKey("fk_hotel_rooms_hotel", "hotels", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("room_type").AsString(50).NotNullable()
                .WithColumn("capacity").AsInt32().NotNullable()
                .WithColumn("total_rooms").AsInt32().NotNullable()
                .WithColumn("price_per_night").AsDecimal(10, 2).NotNullable()
                .WithColumn("features").AsString(int.MaxValue).Nullable() // JSON array
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true);

            Create.Index("idx_hotel_rooms_hotel")
                .OnTable("hotel_rooms")
                .OnColumn("hotel_id").Ascending();

            // ============================================================
            // VEHICLES
            // ============================================================
            Create.Table("vehicles")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("vehicle_type").AsString(50).NotNullable()
                .WithColumn("model").AsString(100).NotNullable()
                .WithColumn("registration_number").AsString(50).NotNullable().Unique()
                .WithColumn("capacity").AsInt32().NotNullable()
                .WithColumn("features").AsString(int.MaxValue).Nullable() // JSON array
                .WithColumn("price_per_day").AsDecimal(10, 2).NotNullable()
                .WithColumn("price_per_km").AsDecimal(10, 2).Nullable()
                .WithColumn("driver_name").AsString(100).Nullable()
                .WithColumn("driver_contact").AsString(20).Nullable()
                .WithColumn("insurance_number").AsString(100).Nullable()
                .WithColumn("insurance_expiry").AsDate().Nullable()
                .WithColumn("permit_number").AsString(100).Nullable()
                .WithColumn("permit_expiry").AsDate().Nullable()
                .WithColumn("description").AsString(int.MaxValue).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable()
                .WithColumn("created_by").AsInt32().NotNullable();

            Create.Index("idx_vehicles_type")
                .OnTable("vehicles")
                .OnColumn("vehicle_type").Ascending();

            Create.Index("idx_vehicles_capacity")
                .OnTable("vehicles")
                .OnColumn("capacity").Ascending();

            Create.Index("idx_vehicles_active")
                .OnTable("vehicles")
                .OnColumn("is_active").Ascending();

            // ============================================================
            // GUIDES
            // ============================================================
            Create.Table("guides")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("full_name").AsString(100).NotNullable()
                .WithColumn("phone").AsString(20).Nullable()
                .WithColumn("email").AsString(100).Nullable()
                .WithColumn("address").AsString(500).Nullable()
                .WithColumn("experience_years").AsInt32().NotNullable()
                .WithColumn("languages").AsString(int.MaxValue).Nullable() // JSON array
                .WithColumn("specialization").AsString(100).Nullable()
                .WithColumn("certification_number").AsString(100).Nullable()
                .WithColumn("certification_expiry").AsDate().Nullable()
                .WithColumn("license_number").AsString(100).Nullable()
                .WithColumn("price_per_day").AsDecimal(10, 2).NotNullable()
                .WithColumn("rating").AsDecimal(3, 2).Nullable()
                .WithColumn("bio").AsString(int.MaxValue).Nullable()
                .WithColumn("photo").AsString(500).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable()
                .WithColumn("created_by").AsInt32().NotNullable();

            Create.Index("idx_guides_specialization")
                .OnTable("guides")
                .OnColumn("specialization").Ascending();

            Create.Index("idx_guides_active")
                .OnTable("guides")
                .OnColumn("is_active").Ascending();

            // ============================================================
            // ACTIVITIES
            // ============================================================
            Create.Table("activities")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("activity_type").AsString(50).NotNullable()
                .WithColumn("location").AsString(100).Nullable()
                .WithColumn("duration_hours").AsInt32().NotNullable()
                .WithColumn("difficulty_level").AsString(20).NotNullable()
                .WithColumn("max_participants").AsInt32().NotNullable()
                .WithColumn("min_participants").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("equipment").AsString(int.MaxValue).Nullable() // JSON array
                .WithColumn("price_per_person").AsDecimal(10, 2).NotNullable()
                .WithColumn("description").AsString(int.MaxValue).Nullable()
                .WithColumn("safety_instructions").AsString(int.MaxValue).Nullable()
                .WithColumn("images").AsString(int.MaxValue).Nullable() // JSON array
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable()
                .WithColumn("created_by").AsInt32().NotNullable();

            Create.Index("idx_activities_type")
                .OnTable("activities")
                .OnColumn("activity_type").Ascending();

            Create.Index("idx_activities_location")
                .OnTable("activities")
                .OnColumn("location").Ascending();

            Create.Index("idx_activities_active")
                .OnTable("activities")
                .OnColumn("is_active").Ascending();
        }

        public override void Down()
        {
            Delete.Table("activities");
            Delete.Table("guides");
            Delete.Table("vehicles");
            Delete.Table("hotel_rooms");
            Delete.Table("hotels");
        }
    }
}
