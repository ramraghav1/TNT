using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202604160001)]
    public class AddItineraryProposalTables : Migration
    {
        public override void Up()
        {
            // ============================================================
            // 1. Itinerary Proposals
            // ============================================================
            Create.Table("itinerary_proposals")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("itinerary_id").AsInt64().NotNullable()
                .WithColumn("token").AsString(64).NotNullable().Unique()
                .WithColumn("traveler_name").AsString(300).NotNullable()
                .WithColumn("traveler_email").AsString(300).NotNullable()
                .WithColumn("traveler_phone").AsString(50).Nullable()
                .WithColumn("status").AsString(50).NotNullable().WithDefaultValue("Sent")
                .WithColumn("start_date").AsDate().NotNullable()
                .WithColumn("end_date").AsDate().NotNullable()
                .WithColumn("total_amount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("notes").AsString(int.MaxValue).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("expires_at").AsDateTime().NotNullable();

            Create.Index("ix_itinerary_proposals_token")
                .OnTable("itinerary_proposals")
                .OnColumn("token").Ascending()
                .WithOptions().Unique();

            Create.Index("ix_itinerary_proposals_itinerary_id")
                .OnTable("itinerary_proposals")
                .OnColumn("itinerary_id").Ascending();

            // ============================================================
            // 2. Itinerary Proposal Days
            // ============================================================
            Create.Table("itinerary_proposal_days")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("proposal_id").AsInt64().NotNullable()
                .WithColumn("day_number").AsInt32().NotNullable()
                .WithColumn("title").AsString(300).Nullable()
                .WithColumn("location").AsString(300).Nullable()
                .WithColumn("accommodation").AsString(300).Nullable()
                .WithColumn("transport").AsString(300).Nullable()
                .WithColumn("breakfast_included").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("lunch_included").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("dinner_included").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("activities").AsString(int.MaxValue).Nullable()
                .WithColumn("daily_cost").AsDecimal(18, 2).NotNullable().WithDefaultValue(0);

            Create.Index("ix_itinerary_proposal_days_proposal_id")
                .OnTable("itinerary_proposal_days")
                .OnColumn("proposal_id").Ascending();

            // ============================================================
            // 3. Itinerary Proposal Payments
            // ============================================================
            Create.Table("itinerary_proposal_payments")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("proposal_id").AsInt64().NotNullable()
                .WithColumn("payment_type").AsString(50).NotNullable().WithDefaultValue("Full")
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("screenshot_path").AsString(500).Nullable()
                .WithColumn("transaction_reference").AsString(200).Nullable()
                .WithColumn("status").AsString(50).NotNullable().WithDefaultValue("Pending")
                .WithColumn("verified_by").AsString(200).Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("verified_at").AsDateTime().Nullable();

            Create.Index("ix_itinerary_proposal_payments_proposal_id")
                .OnTable("itinerary_proposal_payments")
                .OnColumn("proposal_id").Ascending();

            // ============================================================
            // 4. Itinerary Proposal Feedback
            // ============================================================
            Create.Table("itinerary_proposal_feedback")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("proposal_id").AsInt64().NotNullable()
                .WithColumn("message").AsString(int.MaxValue).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("ix_itinerary_proposal_feedback_proposal_id")
                .OnTable("itinerary_proposal_feedback")
                .OnColumn("proposal_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("itinerary_proposal_feedback");
            Delete.Table("itinerary_proposal_payments");
            Delete.Table("itinerary_proposal_days");
            Delete.Table("itinerary_proposals");
        }
    }
}
