using System;
using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(2026022102)]
    public class AddItineraryCoreTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1️⃣ TEMPLATE: Itineraries
            // ============================================================
            Create.Table("Itineraries")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Title").AsString(300).NotNullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("DurationDays").AsInt32().NotNullable()
                .WithColumn("DifficultyLevel").AsString(100).Nullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedBy").AsString(150).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            // ============================================================
            // 2️⃣ TEMPLATE: Itinerary Days
            // ============================================================
            Create.Table("ItineraryDays")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ItineraryId").AsInt64().NotNullable()
                .ForeignKey("FK_ItineraryDays_Itineraries", "Itineraries", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("DayNumber").AsInt32().NotNullable()
                .WithColumn("Title").AsString(200).Nullable()
                .WithColumn("Location").AsString(200).Nullable()
                .WithColumn("Accommodation").AsString(200).Nullable()
                .WithColumn("Transport").AsString(150).Nullable()
                .WithColumn("BreakfastIncluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("LunchIncluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("DinnerIncluded").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Index("IX_ItineraryDays_ItineraryId")
                .OnTable("ItineraryDays")
                .OnColumn("ItineraryId").Ascending();

            // ============================================================
            // 3️⃣ TEMPLATE: Day Activities
            // ============================================================
            Create.Table("ItineraryDayActivities")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ItineraryDayId").AsInt64().NotNullable()
                .ForeignKey("FK_Activities_ItineraryDays", "ItineraryDays", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Activity").AsString(500).NotNullable();

            Create.Index("IX_Activities_ItineraryDayId")
                .OnTable("ItineraryDayActivities")
                .OnColumn("ItineraryDayId").Ascending();

            // ============================================================
            // 4️⃣ INSTANCE: Itinerary Instance (Customer Copy)
            // ============================================================
            Create.Table("ItineraryInstances")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("TemplateItineraryId").AsInt64().NotNullable()
                .ForeignKey("FK_Instance_Template", "Itineraries", "Id")
                .WithColumn("Status").AsString(50).NotNullable() // Draft, Customized, Accepted, Confirmed
                .WithColumn("StartDate").AsDate().Nullable()
                .WithColumn("EndDate").AsDate().Nullable()
                .WithColumn("TotalPrice").AsDecimal(18, 2).Nullable()
                .WithColumn("Currency").AsString(20).Nullable()
                .WithColumn("IsCustomized").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);


            Create.Index("IX_ItineraryInstances_TemplateId")
                .OnTable("ItineraryInstances")
                .OnColumn("TemplateItineraryId").Ascending();

            Alter.Table("ItineraryInstances")
    .AddColumn("BookingReference").AsString(100).Nullable()
    .AddColumn("PaymentStatus").AsString(50).NotNullable().WithDefaultValue("Unpaid")
    .AddColumn("TotalAmount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
    .AddColumn("AmountPaid").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
    .AddColumn("BalanceAmount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
    .AddColumn("TravelerApproved").AsBoolean().NotNullable().WithDefaultValue(false)
    .AddColumn("AdminApproved").AsBoolean().NotNullable().WithDefaultValue(false)
    .AddColumn("ConfirmedAt").AsDateTime().Nullable();

            // ============================================================
            // 5️⃣ INSTANCE: Instance Days
            // ============================================================
            Create.Table("ItineraryInstanceDays")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ItineraryInstanceId").AsInt64().NotNullable()
                .ForeignKey("FK_InstanceDays_Instance", "ItineraryInstances", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("DayNumber").AsInt32().NotNullable()
                .WithColumn("Date").AsDate().Nullable()
                .WithColumn("Title").AsString(200).Nullable()
                .WithColumn("Location").AsString(200).Nullable()
                .WithColumn("Accommodation").AsString(200).Nullable()
                .WithColumn("Transport").AsString(150).Nullable()
                .WithColumn("BreakfastIncluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("LunchIncluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("DinnerIncluded").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Index("IX_InstanceDays_InstanceId")
                .OnTable("ItineraryInstanceDays")
                .OnColumn("ItineraryInstanceId").Ascending();

            // ============================================================
            // 6️⃣ INSTANCE: Day Activities
            // ============================================================
            Create.Table("ItineraryInstanceDayActivities")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("InstanceDayId").AsInt64().NotNullable()
                .ForeignKey("FK_InstanceActivities_InstanceDay", "ItineraryInstanceDays", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Activity").AsString(500).NotNullable();

            Create.Index("IX_InstanceActivities_InstanceDayId")
                .OnTable("ItineraryInstanceDayActivities")
                .OnColumn("InstanceDayId").Ascending();

            // ============================================================
            // 7️⃣ Travelers (Added After Accept)
            // ============================================================
            Create.Table("Travelers")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ItineraryInstanceId").AsInt64().NotNullable()
                .ForeignKey("FK_Travelers_Instance", "ItineraryInstances", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("FullName").AsString(200).NotNullable()
                .WithColumn("ContactNumber").AsString(50).Nullable()
                .WithColumn("Email").AsString(200).Nullable()
                .WithColumn("Nationality").AsString(100).Nullable()
                .WithColumn("Adults").AsInt32().NotNullable()
                .WithColumn("Children").AsInt32().NotNullable()
                .WithColumn("Seniors").AsInt32().NotNullable();

            Create.Index("IX_Travelers_InstanceId")
                .OnTable("Travelers")
                .OnColumn("ItineraryInstanceId").Ascending();

            Create.Table("ItineraryApprovals")
    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
    .WithColumn("ItineraryInstanceId").AsInt64().NotNullable()
    .ForeignKey("FK_Approval_Instance", "ItineraryInstances", "Id")
    .WithColumn("ApprovedBy").AsString(100).NotNullable() // Traveler/Admin
    .WithColumn("Approved").AsBoolean().NotNullable()
    .WithColumn("Remarks").AsString(500).Nullable()
    .WithColumn("ApprovedAt").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Travelers");
            Delete.Table("ItineraryInstanceDayActivities");
            Delete.Table("ItineraryInstanceDays");
            Delete.Table("ItineraryInstances");
            Delete.Table("ItineraryDayActivities");
            Delete.Table("ItineraryDays");
            Delete.Table("Itineraries");
        }
    }
}

