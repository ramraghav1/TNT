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
            Create.Table("costitems")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()          // Breakfast, Porter
                .WithColumn("category").AsString(100).NotNullable()     // Meal, Transport, Permit
                .WithColumn("unittype").AsString(50).NotNullable()      // PerPerson, PerDay, Fixed
                .WithColumn("isactive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("createdat").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_costitems_name")
                .OnTable("costitems")
                .OnColumn("name").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 2️⃣ COST ITEM RATES (Flexible Pricing Rules)
            // ============================================================
            Create.Table("costitemrates")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("costitemid").AsInt64().NotNullable()
                .ForeignKey("fk_costrates_costitems", "costitems", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("location").AsString(200).Nullable()     // Optional
                .WithColumn("itineraryid").AsInt64().Nullable()
                .ForeignKey("fk_costrates_itinerary", "itineraries", "id")
                .WithColumn("price").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(20).NotNullable()
                .WithColumn("effectivefrom").AsDate().Nullable()
                .WithColumn("effectiveto").AsDate().Nullable();

            Create.Index("ix_costrates_costitemid")
                .OnTable("costitemrates")
                .OnColumn("costitemid").Ascending();

            // ============================================================
            // 3️⃣ TEMPLATE DAY COST MAPPING
            // ============================================================
            Create.Table("itinerarydaycosts")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerarydayid").AsInt64().NotNullable()
                .ForeignKey("fk_daycosts_itineraryday", "itinerarydays", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("costitemid").AsInt64().NotNullable()
                .ForeignKey("fk_daycosts_costitem", "costitems", "id")
                .WithColumn("quantity").AsInt32().NotNullable().WithDefaultValue(1);

            Create.Index("ix_daycosts_dayid")
                .OnTable("itinerarydaycosts")
                .OnColumn("itinerarydayid").Ascending();

            // ============================================================
            // 4️⃣ INSTANCE DAY COST SNAPSHOT
            // ============================================================
            Create.Table("itineraryinstancedaycosts")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("instancedayid").AsInt64().NotNullable()
                .ForeignKey("fk_instancedaycosts_instanceday", "itineraryinstancedays", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("costitemid").AsInt64().NotNullable()
                .ForeignKey("fk_instancedaycosts_costitem", "costitems", "id")
                .WithColumn("unitprice").AsDecimal(18, 2).NotNullable()
                .WithColumn("quantity").AsInt32().NotNullable()
                .WithColumn("totalprice").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(20).NotNullable();

            Create.Index("ix_instancedaycosts_instancedayid")
                .OnTable("itineraryinstancedaycosts")
                .OnColumn("instancedayid").Ascending();

            // ============================================================
            // 5️⃣ PAYMENTS
            // ============================================================
            Create.Table("payments")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itineraryinstanceid").AsInt64().NotNullable()
                .ForeignKey("fk_payments_instance", "itineraryinstances", "id")
                .WithColumn("paymentmethod").AsString(100).NotNullable() // eSewa, Card, Bank
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(20).NotNullable()
                .WithColumn("paymentdate").AsDateTime().NotNullable()
                .WithColumn("transactionreference").AsString(200).Nullable()
                .WithColumn("status").AsString(50).NotNullable(); // Success, Failed, Pending
        }

        public override void Down()
        {
            Delete.Table("payments");
            Delete.Table("itineraryinstancedaycosts");
            Delete.Table("itinerarydaycosts");
            Delete.Table("costitemrates");
            Delete.Table("costitems");
        }
    }
}