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
            // 1️⃣ TEMPLATE: Itineraries
            // ============================================================
            Create.Table("itineraries")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("title").AsString(300).NotNullable()
                .WithColumn("description").AsString(int.MaxValue).Nullable()
                .WithColumn("durationdays").AsInt32().NotNullable()
                .WithColumn("difficultylevel").AsString(100).Nullable()
                .WithColumn("isactive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("createdby").AsString(150).Nullable()
                .WithColumn("createdat").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            // ============================================================
            // 2️⃣ TEMPLATE: Itinerary Days
            // ============================================================
            Create.Table("itinerarydays")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itineraryid").AsInt64().NotNullable()
                .ForeignKey("fk_itinerarydays_itineraries", "itineraries", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("daynumber").AsInt32().NotNullable()
                .WithColumn("title").AsString(200).Nullable()
                .WithColumn("location").AsString(200).Nullable()
                .WithColumn("accommodation").AsString(200).Nullable()
                .WithColumn("transport").AsString(150).Nullable()
                .WithColumn("breakfastincluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("lunchincluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("dinnerincluded").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Index("ix_itinerarydays_itineraryid")
                .OnTable("itinerarydays")
                .OnColumn("itineraryid").Ascending();

            // ============================================================
            // 3️⃣ TEMPLATE: Day Activities
            // ============================================================
            Create.Table("itinerarydayactivities")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerarydayid").AsInt64().NotNullable()
                .ForeignKey("fk_activities_itinerarydays", "itinerarydays", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("activity").AsString(500).NotNullable();

            Create.Index("ix_activities_itinerarydayid")
                .OnTable("itinerarydayactivities")
                .OnColumn("itinerarydayid").Ascending();

            // ============================================================
            // 4️⃣ INSTANCE: Itinerary Instance (Customer Copy)
            // ============================================================
            Create.Table("itineraryinstances")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("templateitineraryid").AsInt64().NotNullable()
                .ForeignKey("fk_instance_template", "itineraries", "id")
                .WithColumn("status").AsString(50).NotNullable() // Draft, Customized, Accepted, Confirmed
                .WithColumn("startdate").AsDate().Nullable()
                .WithColumn("enddate").AsDate().Nullable()
                .WithColumn("totalprice").AsDecimal(18, 2).Nullable()
                .WithColumn("currency").AsString(20).Nullable()
                .WithColumn("iscustomized").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("createdat").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_itineraryinstances_templateid")
                .OnTable("itineraryinstances")
                .OnColumn("templateitineraryid").Ascending();

            Alter.Table("itineraryinstances")
                .AddColumn("bookingreference").AsString(100).Nullable()
                .AddColumn("paymentstatus").AsString(50).NotNullable().WithDefaultValue("Unpaid")
                .AddColumn("totalamount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .AddColumn("amountpaid").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .AddColumn("balanceamount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .AddColumn("travelerapproved").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("adminapproved").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("confirmedat").AsDateTime().Nullable();

            // ============================================================
            // 5️⃣ INSTANCE: Instance Days
            // ============================================================
            Create.Table("itineraryinstancedays")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itineraryinstanceid").AsInt64().NotNullable()
                .ForeignKey("fk_instancedays_instance", "itineraryinstances", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("daynumber").AsInt32().NotNullable()
                .WithColumn("date").AsDate().Nullable()
                .WithColumn("title").AsString(200).Nullable()
                .WithColumn("location").AsString(200).Nullable()
                .WithColumn("accommodation").AsString(200).Nullable()
                .WithColumn("transport").AsString(150).Nullable()
                .WithColumn("breakfastincluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("lunchincluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("dinnerincluded").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Index("ix_instancedays_instanceid")
                .OnTable("itineraryinstancedays")
                .OnColumn("itineraryinstanceid").Ascending();

            // ============================================================
            // 6️⃣ INSTANCE: Day Activities
            // ============================================================
            Create.Table("itineraryinstancedayactivities")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("instancedayid").AsInt64().NotNullable()
                .ForeignKey("fk_instanceactivities_instanceday", "itineraryinstancedays", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("activity").AsString(500).NotNullable();

            Create.Index("ix_instanceactivities_instancedayid")
                .OnTable("itineraryinstancedayactivities")
                .OnColumn("instancedayid").Ascending();

            // ============================================================
            // 7️⃣ Travelers (Added After Accept)
            // ============================================================
            Create.Table("travelers")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itineraryinstanceid").AsInt64().NotNullable()
                .ForeignKey("fk_travelers_instance", "itineraryinstances", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("fullname").AsString(200).NotNullable()
                .WithColumn("contactnumber").AsString(50).Nullable()
                .WithColumn("email").AsString(200).Nullable()
                .WithColumn("nationality").AsString(100).Nullable()
                .WithColumn("adults").AsInt32().NotNullable()
                .WithColumn("children").AsInt32().NotNullable()
                .WithColumn("seniors").AsInt32().NotNullable();

            Create.Index("ix_travelers_instanceid")
                .OnTable("travelers")
                .OnColumn("itineraryinstanceid").Ascending();

            Create.Table("itineraryapprovals")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itineraryinstanceid").AsInt64().NotNullable()
                .ForeignKey("fk_approval_instance", "itineraryinstances", "id")
                .WithColumn("approvedby").AsString(100).NotNullable()
                .WithColumn("approved").AsBoolean().NotNullable()
                .WithColumn("remarks").AsString(500).Nullable()
                .WithColumn("approvedat").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("travelers");
            Delete.Table("itineraryinstancedayactivities");
            Delete.Table("itineraryinstancedays");
            Delete.Table("itineraryinstances");
            Delete.Table("itinerarydayactivities");
            Delete.Table("itinerarydays");
            Delete.Table("itineraries");
        }
    }
}