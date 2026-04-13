using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202609010001)]
    public class AddFinanceTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1. TAX CONFIGURATION (per tenant — VAT/PAN Nepal)
            // ============================================================
            Create.Table("tnt_tax_configs")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable()
                .WithColumn("tax_name").AsString(100).NotNullable()           // e.g. "VAT", "Service Tax"
                .WithColumn("tax_rate").AsDecimal(5, 2).NotNullable()         // e.g. 13.00 for 13% VAT Nepal
                .WithColumn("tax_type").AsString(20).NotNullable()            // "Percentage" | "Fixed"
                .WithColumn("pan_number").AsString(50).Nullable()             // PAN number for Nepal compliance
                .WithColumn("vat_number").AsString(50).Nullable()             // VAT registration number
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_tnt_tax_configs_tenant_id")
                .OnTable("tnt_tax_configs").OnColumn("tenant_id").Ascending();

            // ============================================================
            // 2. INVOICES (auto-generated per booking)
            // ============================================================
            Create.Table("tnt_invoices")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable()
                .WithColumn("booking_id").AsInt64().NotNullable()              // FK → itinerary_instances.id
                .ForeignKey("fk_tnt_invoices_booking", "itinerary_instances", "id")
                .WithColumn("invoice_number").AsString(50).NotNullable()       // e.g. INV-2026-00001
                .WithColumn("invoice_date").AsDate().NotNullable()
                .WithColumn("due_date").AsDate().Nullable()
                .WithColumn("status").AsString(30).NotNullable().WithDefaultValue("Draft")
                    // Draft | Sent | Paid | PartiallyPaid | Overdue | Cancelled | Refunded
                .WithColumn("currency").AsString(10).NotNullable().WithDefaultValue("NPR")
                .WithColumn("exchange_rate").AsDecimal(18, 6).NotNullable().WithDefaultValue(1)  // to NPR
                .WithColumn("subtotal").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("discount_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("tax_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("total_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("amount_paid").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("balance_due").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                // Customer snapshot (denormalised for immutability)
                .WithColumn("customer_name").AsString(200).NotNullable()
                .WithColumn("customer_email").AsString(200).Nullable()
                .WithColumn("customer_phone").AsString(50).Nullable()
                .WithColumn("customer_address").AsString(500).Nullable()
                // Nepal compliance
                .WithColumn("pan_number").AsString(50).Nullable()
                .WithColumn("vat_number").AsString(50).Nullable()
                .WithColumn("notes").AsString(int.MaxValue).Nullable()
                .WithColumn("created_by").AsString(150).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("ix_tnt_invoices_booking_id")
                .OnTable("tnt_invoices").OnColumn("booking_id").Ascending();
            Create.Index("ix_tnt_invoices_tenant_id")
                .OnTable("tnt_invoices").OnColumn("tenant_id").Ascending();
            Create.Index("ix_tnt_invoices_number")
                .OnTable("tnt_invoices").OnColumn("invoice_number").Ascending();

            // ============================================================
            // 3. INVOICE LINE ITEMS
            // ============================================================
            Create.Table("tnt_invoice_line_items")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("invoice_id").AsInt64().NotNullable()
                .ForeignKey("fk_tnt_line_items_invoice", "tnt_invoices", "id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("item_type").AsString(50).NotNullable()           // Package|Hotel|Guide|Transport|Activity|Tax|Discount|Other
                .WithColumn("description").AsString(500).NotNullable()
                .WithColumn("quantity").AsDecimal(10, 2).NotNullable().WithDefaultValue(1)
                .WithColumn("unit_price").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("discount_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("tax_rate").AsDecimal(5, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("tax_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)  // net after tax/discount
                .WithColumn("sort_order").AsInt32().NotNullable().WithDefaultValue(0);

            Create.Index("ix_tnt_line_items_invoice_id")
                .OnTable("tnt_invoice_line_items").OnColumn("invoice_id").Ascending();

            // ============================================================
            // 4. AGENT COMMISSIONS
            // ============================================================
            Create.Table("tnt_agent_commissions")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable()
                .WithColumn("booking_id").AsInt64().NotNullable()
                .ForeignKey("fk_tnt_commissions_booking", "itinerary_instances", "id")
                .WithColumn("agent_name").AsString(200).NotNullable()
                .WithColumn("agent_contact").AsString(100).Nullable()
                .WithColumn("commission_type").AsString(20).NotNullable()     // "Percentage" | "Fixed"
                .WithColumn("commission_rate").AsDecimal(5, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("commission_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("currency").AsString(10).NotNullable().WithDefaultValue("NPR")
                .WithColumn("status").AsString(20).NotNullable().WithDefaultValue("Pending")
                    // Pending | Paid | Cancelled
                .WithColumn("payment_date").AsDate().Nullable()
                .WithColumn("notes").AsString(500).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_tnt_commissions_booking_id")
                .OnTable("tnt_agent_commissions").OnColumn("booking_id").Ascending();
            Create.Index("ix_tnt_commissions_tenant_id")
                .OnTable("tnt_agent_commissions").OnColumn("tenant_id").Ascending();

            // ============================================================
            // 5. EXPENSES (actual costs incurred per booking)
            // ============================================================
            Create.Table("tnt_expenses")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable()
                .WithColumn("booking_id").AsInt64().Nullable()                // nullable = general expense not tied to booking
                .ForeignKey("fk_tnt_expenses_booking", "itinerary_instances", "id")
                .WithColumn("category").AsString(50).NotNullable()            // Hotel|Guide|Transport|Activity|Food|Misc
                .WithColumn("description").AsString(500).NotNullable()
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("currency").AsString(10).NotNullable().WithDefaultValue("NPR")
                .WithColumn("expense_date").AsDate().NotNullable()
                .WithColumn("paid_to").AsString(200).Nullable()               // vendor/provider name
                .WithColumn("payment_method").AsString(50).Nullable()         // Cash|Bank|eSewa|Card
                .WithColumn("reference_number").AsString(100).Nullable()
                .WithColumn("receipt_url").AsString(500).Nullable()
                .WithColumn("notes").AsString(500).Nullable()
                .WithColumn("created_by").AsString(150).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_tnt_expenses_booking_id")
                .OnTable("tnt_expenses").OnColumn("booking_id").Ascending();
            Create.Index("ix_tnt_expenses_tenant_id")
                .OnTable("tnt_expenses").OnColumn("tenant_id").Ascending();

            // ============================================================
            // 6. REFUNDS
            // ============================================================
            Create.Table("tnt_refunds")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("tenant_id").AsInt64().NotNullable()
                .WithColumn("booking_id").AsInt64().NotNullable()
                .ForeignKey("fk_tnt_refunds_booking", "itinerary_instances", "id")
                .WithColumn("refund_number").AsString(50).NotNullable()        // RFN-2026-00001
                .WithColumn("original_amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("refund_amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("cancellation_fee").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("currency").AsString(10).NotNullable().WithDefaultValue("NPR")
                .WithColumn("reason").AsString(500).NotNullable()
                .WithColumn("status").AsString(20).NotNullable().WithDefaultValue("Pending")
                    // Pending | Processed | Rejected
                .WithColumn("refund_method").AsString(50).Nullable()           // Bank|eSewa|Cash|Card
                .WithColumn("transaction_reference").AsString(100).Nullable()
                .WithColumn("processed_at").AsDateTime().Nullable()
                .WithColumn("processed_by").AsString(150).Nullable()
                .WithColumn("notes").AsString(500).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_tnt_refunds_booking_id")
                .OnTable("tnt_refunds").OnColumn("booking_id").Ascending();
            Create.Index("ix_tnt_refunds_tenant_id")
                .OnTable("tnt_refunds").OnColumn("tenant_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("tnt_refunds");
            Delete.Table("tnt_expenses");
            Delete.Table("tnt_agent_commissions");
            Delete.Table("tnt_invoice_line_items");
            Delete.Table("tnt_invoices");
            Delete.Table("tnt_tax_configs");
        }
    }
}
