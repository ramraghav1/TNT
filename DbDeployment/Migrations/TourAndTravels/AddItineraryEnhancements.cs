using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    /// <summary>
    /// Adds media storage, map coordinates, and booking assignments for itineraries
    /// - Media (photos, videos) for marketing
    /// - Map coordinates for displaying route
    /// - Hotel and guide assignments to instance days
    /// - Availability tracking
    /// </summary>
    [Migration(202604030001)]
    public class AddItineraryEnhancements : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1. Add Media & Map Fields to Template Itineraries
            // ============================================================
            if (!Schema.Table("itineraries").Column("thumbnail_url").Exists())
            {
                Alter.Table("itineraries")
                    .AddColumn("thumbnail_url").AsString(500).Nullable()
                    .AddColumn("map_image_url").AsString(500).Nullable()
                    .AddColumn("map_embed_url").AsString(1000).Nullable() // Google Maps embed
                    .AddColumn("map_coordinates").AsString(1000).Nullable() // JSON array of lat/lng
                    .AddColumn("is_published").AsBoolean().NotNullable().WithDefaultValue(false);
            }

            // ============================================================
            // 2. Itinerary Media (Photos & Videos)
            // ============================================================
            Create.Table("itinerary_media")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_id").AsInt64().NotNullable()
                    .ForeignKey("fk_media_itinerary", "itineraries", "id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("media_type").AsString(50).NotNullable() // 'Image', 'Video', 'VideoLink'
                .WithColumn("media_url").AsString(1000).NotNullable()
                .WithColumn("caption").AsString(500).Nullable()
                .WithColumn("display_order").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("is_featured").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_itinerary_media_itinerary_id")
                .OnTable("itinerary_media")
                .OnColumn("itinerary_id").Ascending();

            // ============================================================
            // 3. Instance Day Assignments (Hotels & Guides)
            // ============================================================
            Create.Table("itinerary_instance_day_assignments")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("instance_day_id").AsInt64().NotNullable()
                    .ForeignKey("fk_assignments_instance_day", "itinerary_instance_days", "id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("hotel_id").AsInt64().Nullable()
                    .ForeignKey("fk_assignments_hotel", "hotels", "id")
                    .OnDelete(System.Data.Rule.SetNull)
                .WithColumn("room_type").AsString(100).Nullable()
                .WithColumn("number_of_rooms").AsInt32().Nullable()
                .WithColumn("guide_id").AsInt64().Nullable()
                    .ForeignKey("fk_assignments_guide", "guides", "id")
                    .OnDelete(System.Data.Rule.SetNull)
                .WithColumn("vehicle_id").AsInt64().Nullable()
                    .ForeignKey("fk_assignments_vehicle", "vehicles", "id")
                    .OnDelete(System.Data.Rule.SetNull)
                .WithColumn("notes").AsString(1000).Nullable()
                .WithColumn("assigned_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("assigned_by").AsString(150).Nullable();

            Create.Index("ix_day_assignments_instance_day")
                .OnTable("itinerary_instance_day_assignments")
                .OnColumn("instance_day_id").Ascending();

            Create.Index("ix_day_assignments_hotel")
                .OnTable("itinerary_instance_day_assignments")
                .OnColumn("hotel_id").Ascending();

            Create.Index("ix_day_assignments_guide")
                .OnTable("itinerary_instance_day_assignments")
                .OnColumn("guide_id").Ascending();

            Create.Index("ix_day_assignments_vehicle")
                .OnTable("itinerary_instance_day_assignments")
                .OnColumn("vehicle_id").Ascending();

            // ============================================================
            // 4. Add Date & Assignment Status to Instance Days
            // ============================================================
            if (!Schema.Table("itinerary_instance_days").Column("assignment_status").Exists())
            {
                Alter.Table("itinerary_instance_days")
                    .AddColumn("assignment_status").AsString(50).NotNullable()
                        .WithDefaultValue("Pending") // Pending, Confirmed, Conflict
                    .AddColumn("assignment_notes").AsString(1000).Nullable();
            }

            // ============================================================
            // 5. Booking Requests (from website travelers)
            // ============================================================
            Create.Table("booking_requests")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_id").AsInt64().NotNullable()
                    .ForeignKey("fk_booking_request_itinerary", "itineraries", "id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("customer_name").AsString(200).NotNullable()
                .WithColumn("customer_email").AsString(200).NotNullable()
                .WithColumn("customer_phone").AsString(50).Nullable()
                .WithColumn("preferred_start_date").AsDate().Nullable()
                .WithColumn("number_of_travelers").AsInt32().NotNullable()
                .WithColumn("message").AsString(2000).Nullable()
                .WithColumn("status").AsString(50).NotNullable().WithDefaultValue("Pending") // Pending, Contacted, Converted, Rejected
                .WithColumn("converted_to_instance_id").AsInt64().Nullable()
                    .ForeignKey("fk_booking_request_converted", "itinerary_instances", "id")
                    .OnDelete(System.Data.Rule.SetNull)
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("responded_at").AsDateTime().Nullable()
                .WithColumn("responded_by").AsString(150).Nullable();

            Create.Index("ix_booking_requests_itinerary")
                .OnTable("booking_requests")
                .OnColumn("itinerary_id").Ascending();

            Create.Index("ix_booking_requests_status")
                .OnTable("booking_requests")
                .OnColumn("status").Ascending();

            // ============================================================
            // 6. Add Search/SEO Fields to Itineraries
            // ============================================================
            if (!Schema.Table("itineraries").Column("short_description").Exists())
            {
                Alter.Table("itineraries")
                    .AddColumn("short_description").AsString(500).Nullable() // For cards/summaries
                    .AddColumn("highlights").AsString(2000).Nullable() // Key selling points
                    .AddColumn("price_starting_from").AsDecimal(18, 2).Nullable()
                    .AddColumn("season_best_time").AsString(200).Nullable() // e.g., "March-May, September-November"
                    .AddColumn("max_group_size").AsInt32().Nullable()
                    .AddColumn("min_age_requirement").AsInt32().Nullable()
                    .AddColumn("tags").AsString(500).Nullable(); // Comma-separated: "adventure,trekking,mountains"
            }
        }

        public override void Down()
        {
            // Drop in reverse order
            Delete.Table("booking_requests");
            Delete.Table("itinerary_instance_day_assignments");
            Delete.Table("itinerary_media");

            // Remove new columns
            if (Schema.Table("itineraries").Column("thumbnail_url").Exists())
            {
                Delete.Column("thumbnail_url").FromTable("itineraries");
                Delete.Column("map_image_url").FromTable("itineraries");
                Delete.Column("map_embed_url").FromTable("itineraries");
                Delete.Column("map_coordinates").FromTable("itineraries");
                Delete.Column("is_published").FromTable("itineraries");
                Delete.Column("short_description").FromTable("itineraries");
                Delete.Column("highlights").FromTable("itineraries");
                Delete.Column("price_starting_from").FromTable("itineraries");
                Delete.Column("season_best_time").FromTable("itineraries");
                Delete.Column("max_group_size").FromTable("itineraries");
                Delete.Column("min_age_requirement").FromTable("itineraries");
                Delete.Column("tags").FromTable("itineraries");
            }

            if (Schema.Table("itinerary_instance_days").Column("assignment_status").Exists())
            {
                Delete.Column("assignment_status").FromTable("itinerary_instance_days");
                Delete.Column("assignment_notes").FromTable("itinerary_instance_days");
            }
        }
    }
}
