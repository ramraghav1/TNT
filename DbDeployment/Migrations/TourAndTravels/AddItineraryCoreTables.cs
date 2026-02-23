using System;
using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(2026022202)]
    public class AddItineraryCoreTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1. TEMPLATE: Itineraries
            // ============================================================
            Create.Table("itineraries")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("title").AsString(300).NotNullable()
                .WithColumn("description").AsString(int.MaxValue).Nullable()
                .WithColumn("duration_days").AsInt32().NotNullable()
                .WithColumn("difficulty_level").AsString(100).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_by").AsString(150).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            // ============================================================
            // 2. TEMPLATE: Itinerary Days
            // ============================================================
            Create.Table("itinerary_days")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_id").AsInt64().NotNullable()
                .ForeignKey("fk_itinerary_days_itineraries", "itineraries", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("day_number").AsInt32().NotNullable()
                .WithColumn("title").AsString(200).Nullable()
                .WithColumn("location").AsString(200).Nullable()
                .WithColumn("accommodation").AsString(200).Nullable()
                .WithColumn("transport").AsString(150).Nullable()
                .WithColumn("breakfast_included").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("lunch_included").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("dinner_included").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Index("ix_itinerary_days_itinerary_id")
                .OnTable("itinerary_days")
                .OnColumn("itinerary_id").Ascending();

            // ============================================================
            // 3. TEMPLATE: Day Activities
            // ============================================================
            Create.Table("itinerary_day_activities")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_day_id").AsInt64().NotNullable()
                .ForeignKey("fk_activities_itinerary_days", "itinerary_days", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("activity").AsString(500).NotNullable();

            Create.Index("ix_activities_itinerary_day_id")
                .OnTable("itinerary_day_activities")
                .OnColumn("itinerary_day_id").Ascending();

            // ============================================================
            // 4. INSTANCE: Itinerary Instance (Customer Copy)
            // ============================================================
            Create.Table("itinerary_instances")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("template_itinerary_id").AsInt64().NotNullable()
                .ForeignKey("fk_instance_template", "itineraries", "id")
                .WithColumn("source_instance_id").AsInt64().Nullable()
                .WithColumn("status").AsString(50).NotNullable()
                .WithColumn("start_date").AsDate().Nullable()
                .WithColumn("end_date").AsDate().Nullable()
                .WithColumn("total_price").AsDecimal(18, 2).Nullable()
                .WithColumn("currency").AsString(20).Nullable()
                .WithColumn("is_customized").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("special_requests").AsString(int.MaxValue).Nullable()
                .WithColumn("booking_reference").AsString(100).Nullable()
                .WithColumn("payment_status").AsString(50).NotNullable().WithDefaultValue("Unpaid")
                .WithColumn("total_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("amount_paid").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("balance_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("traveler_approved").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("admin_approved").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("confirmed_at").AsDateTime().Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_itinerary_instances_template_id")
                .OnTable("itinerary_instances")
                .OnColumn("template_itinerary_id").Ascending();

            // FK for source_instance_id (self-referencing for reusability)
            Create.ForeignKey("fk_instance_source_instance")
                .FromTable("itinerary_instances").ForeignColumn("source_instance_id")
                .ToTable("itinerary_instances").PrimaryColumn("id");

            // ============================================================
            // 5. INSTANCE: Instance Days
            // ============================================================
            Create.Table("itinerary_instance_days")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_instance_id").AsInt64().NotNullable()
                .ForeignKey("fk_instance_days_instance", "itinerary_instances", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("day_number").AsInt32().NotNullable()
                .WithColumn("date").AsDate().Nullable()
                .WithColumn("title").AsString(200).Nullable()
                .WithColumn("location").AsString(200).Nullable()
                .WithColumn("accommodation").AsString(200).Nullable()
                .WithColumn("transport").AsString(150).Nullable()
                .WithColumn("breakfast_included").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("lunch_included").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("dinner_included").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Index("ix_instance_days_instance_id")
                .OnTable("itinerary_instance_days")
                .OnColumn("itinerary_instance_id").Ascending();

            // ============================================================
            // 6. INSTANCE: Day Activities
            // ============================================================
            Create.Table("itinerary_instance_day_activities")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("instance_day_id").AsInt64().NotNullable()
                .ForeignKey("fk_instance_activities_instance_day", "itinerary_instance_days", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("activity").AsString(500).NotNullable();

            Create.Index("ix_instance_activities_instance_day_id")
                .OnTable("itinerary_instance_day_activities")
                .OnColumn("instance_day_id").Ascending();

            // ============================================================
            // 7. Travelers (Added After Accept)
            // ============================================================
            Create.Table("travelers")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_instance_id").AsInt64().NotNullable()
                .ForeignKey("fk_travelers_instance", "itinerary_instances", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("full_name").AsString(200).NotNullable()
                .WithColumn("contact_number").AsString(50).Nullable()
                .WithColumn("email").AsString(200).Nullable()
                .WithColumn("nationality").AsString(100).Nullable()
                .WithColumn("adults").AsInt32().NotNullable()
                .WithColumn("children").AsInt32().NotNullable()
                .WithColumn("seniors").AsInt32().NotNullable();

            Create.Index("ix_travelers_instance_id")
                .OnTable("travelers")
                .OnColumn("itinerary_instance_id").Ascending();

            Create.Table("itinerary_approvals")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_instance_id").AsInt64().NotNullable()
                .ForeignKey("fk_approval_instance", "itinerary_instances", "id")
                .WithColumn("approved_by").AsString(100).NotNullable()
                .WithColumn("approved").AsBoolean().NotNullable()
                .WithColumn("remarks").AsString(500).Nullable()
                .WithColumn("approved_at").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("itinerary_approvals");
            Delete.Table("travelers");
            Delete.Table("itinerary_instance_day_activities");
            Delete.Table("itinerary_instance_days");
            Delete.Table("itinerary_instances");
            Delete.Table("itinerary_day_activities");
            Delete.Table("itinerary_days");
            Delete.Table("itineraries");
        }
    }
}