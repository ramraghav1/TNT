using System;
using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(2026022103)]
    public class AddItineraryPricingTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1️⃣ COST ITEMS (Master Catalog)
            // ============================================================
            Create.Table("CostItems")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString(200).NotNullable()          // Breakfast, Porter
                .WithColumn("Category").AsString(100).NotNullable()     // Meal, Transport, Permit
                .WithColumn("UnitType").AsString(50).NotNullable()      // PerPerson, PerDay, Fixed
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("IX_CostItems_Name")
                .OnTable("CostItems")
                .OnColumn("Name").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 2️⃣ COST ITEM RATES (Flexible Pricing Rules)
            // ============================================================
            Create.Table("CostItemRates")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("CostItemId").AsInt64().NotNullable()
                .ForeignKey("FK_CostRates_CostItems", "CostItems", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Location").AsString(200).Nullable()     // Optional
                .WithColumn("ItineraryId").AsInt64().Nullable()
                .ForeignKey("FK_CostRates_Itinerary", "Itineraries", "Id")
                .WithColumn("Price").AsDecimal(18, 2).NotNullable()
                .WithColumn("Currency").AsString(20).NotNullable()
                .WithColumn("EffectiveFrom").AsDate().Nullable()
                .WithColumn("EffectiveTo").AsDate().Nullable();

            Create.Index("IX_CostRates_CostItemId")
                .OnTable("CostItemRates")
                .OnColumn("CostItemId").Ascending();

            // ============================================================
            // 3️⃣ TEMPLATE DAY COST MAPPING
            // ============================================================
            Create.Table("ItineraryDayCosts")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ItineraryDayId").AsInt64().NotNullable()
                .ForeignKey("FK_DayCosts_ItineraryDay", "ItineraryDays", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("CostItemId").AsInt64().NotNullable()
                .ForeignKey("FK_DayCosts_CostItem", "CostItems", "Id")
                .WithColumn("Quantity").AsInt32().NotNullable().WithDefaultValue(1);

            Create.Index("IX_DayCosts_DayId")
                .OnTable("ItineraryDayCosts")
                .OnColumn("ItineraryDayId").Ascending();

            // ============================================================
            // 4️⃣ INSTANCE DAY COST SNAPSHOT
            // ============================================================
            Create.Table("ItineraryInstanceDayCosts")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("InstanceDayId").AsInt64().NotNullable()
                .ForeignKey("FK_InstanceDayCosts_InstanceDay", "ItineraryInstanceDays", "Id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("CostItemId").AsInt64().NotNullable()
                .ForeignKey("FK_InstanceDayCosts_CostItem", "CostItems", "Id")
                .WithColumn("UnitPrice").AsDecimal(18, 2).NotNullable()
                .WithColumn("Quantity").AsInt32().NotNullable()
                .WithColumn("TotalPrice").AsDecimal(18, 2).NotNullable()
                .WithColumn("Currency").AsString(20).NotNullable();

            Create.Index("IX_InstanceDayCosts_InstanceDayId")
                .OnTable("ItineraryInstanceDayCosts")
                .OnColumn("InstanceDayId").Ascending();

            Create.Table("Payments")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("ItineraryInstanceId").AsInt64().NotNullable()
            .ForeignKey("FK_Payments_Instance", "ItineraryInstances", "Id")
            .WithColumn("PaymentMethod").AsString(100).NotNullable() // eSewa, Card, Bank
            .WithColumn("Amount").AsDecimal(18, 2).NotNullable()
            .WithColumn("Currency").AsString(20).NotNullable()
            .WithColumn("PaymentDate").AsDateTime().NotNullable()
            .WithColumn("TransactionReference").AsString(200).Nullable()
            .WithColumn("Status").AsString(50).NotNullable(); // Success, Failed, Pending
        }

        public override void Down()
        {
            Delete.Table("ItineraryInstanceDayCosts");
            Delete.Table("ItineraryDayCosts");
            Delete.Table("CostItemRates");
            Delete.Table("CostItems");
        }
    }
}

