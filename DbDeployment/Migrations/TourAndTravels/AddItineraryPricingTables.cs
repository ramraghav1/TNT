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
            // 1. COST ITEMS (Master Catalog)
            // ============================================================
            Create.Table("cost_items")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("category").AsString(100).NotNullable()
                .WithColumn("unit_type").AsString(50).NotNullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_cost_items_name")
                .OnTable("cost_items")
                .OnColumn("name").Ascending()
                .WithOptions().Unique();

            // ============================================================
            // 2. COST ITEM RATES (Flexible Pricing Rules)
            // ============================================================
            Create.Table("cost_item_rates")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("cost_item_id").AsInt64().NotNullable()
                .ForeignKey("fk_cost_rates_cost_items", "cost_items", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("location").AsString(200).Nullable()
                .WithColumn("itinerary_id").AsInt64().Nullable()
                .ForeignKey("fk_cost_rates_itinerary", "itineraries", "id")
                .WithColumn("price").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(20).NotNullable()
                .WithColumn("effective_from").AsDate().Nullable()
                .WithColumn("effective_to").AsDate().Nullable();

            Create.Index("ix_cost_rates_cost_item_id")
                .OnTable("cost_item_rates")
                .OnColumn("cost_item_id").Ascending();

            // ============================================================
            // 3. TEMPLATE DAY COST MAPPING
            // ============================================================
            Create.Table("itinerary_day_costs")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_day_id").AsInt64().NotNullable()
                .ForeignKey("fk_day_costs_itinerary_day", "itinerary_days", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("cost_item_id").AsInt64().NotNullable()
                .ForeignKey("fk_day_costs_cost_item", "cost_items", "id")
                .WithColumn("quantity").AsInt32().NotNullable().WithDefaultValue(1);

            Create.Index("ix_day_costs_day_id")
                .OnTable("itinerary_day_costs")
                .OnColumn("itinerary_day_id").Ascending();

            // ============================================================
            // 4. INSTANCE DAY COST SNAPSHOT
            // ============================================================
            Create.Table("itinerary_instance_day_costs")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("instance_day_id").AsInt64().NotNullable()
                .ForeignKey("fk_instance_day_costs_instance_day", "itinerary_instance_days", "id")
                .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("cost_item_id").AsInt64().NotNullable()
                .ForeignKey("fk_instance_day_costs_cost_item", "cost_items", "id")
                .WithColumn("unit_price").AsDecimal(18, 2).NotNullable()
                .WithColumn("quantity").AsInt32().NotNullable()
                .WithColumn("total_price").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(20).NotNullable();

            Create.Index("ix_instance_day_costs_instance_day_id")
                .OnTable("itinerary_instance_day_costs")
                .OnColumn("instance_day_id").Ascending();

            // ============================================================
            // 5. PAYMENTS
            // ============================================================
            Create.Table("payments")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_instance_id").AsInt64().NotNullable()
                .ForeignKey("fk_payments_instance", "itinerary_instances", "id")
                .WithColumn("payment_method").AsString(100).NotNullable()
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(20).NotNullable()
                .WithColumn("payment_date").AsDateTime().NotNullable()
                .WithColumn("transaction_reference").AsString(200).Nullable()
                .WithColumn("status").AsString(50).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("payments");
            Delete.Table("itinerary_instance_day_costs");
            Delete.Table("itinerary_day_costs");
            Delete.Table("cost_item_rates");
            Delete.Table("cost_items");
        }
    }
}