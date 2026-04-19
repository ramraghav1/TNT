using FluentMigrator;

namespace DbDeployment.Migrations.TourAndTravels
{
    [Migration(202604160003)]
    public class TruncateAllItineraryTables : Migration
    {
        public override void Up()
        {
            // Truncate in reverse dependency order (children first, parents last)

            // Proposal tables
            Execute.Sql("TRUNCATE TABLE itinerary_proposal_feedback CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_proposal_payments CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_proposal_days CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_proposals CASCADE;");

            // Enhancement tables
            Execute.Sql("TRUNCATE TABLE itinerary_instance_day_assignments CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_media CASCADE;");

            // Pricing tables
            Execute.Sql("TRUNCATE TABLE itinerary_instance_day_costs CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_day_costs CASCADE;");
            Execute.Sql("TRUNCATE TABLE cost_item_rates CASCADE;");
            Execute.Sql("TRUNCATE TABLE cost_items CASCADE;");

            // Approval table
            Execute.Sql("TRUNCATE TABLE itinerary_approvals CASCADE;");

            // Instance tables
            Execute.Sql("TRUNCATE TABLE itinerary_instance_day_activities CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_instance_days CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_instances CASCADE;");

            // Core tables
            Execute.Sql("TRUNCATE TABLE itinerary_day_activities CASCADE;");
            Execute.Sql("TRUNCATE TABLE itinerary_days CASCADE;");
            Execute.Sql("TRUNCATE TABLE itineraries CASCADE;");
        }

        public override void Down()
        {
            // No rollback — truncated data cannot be restored
        }
    }
}
