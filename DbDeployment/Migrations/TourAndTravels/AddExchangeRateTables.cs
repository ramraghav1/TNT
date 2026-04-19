using FluentMigrator;

namespace Migrations.TourAndTravels
{
    /// <summary>
    /// Creates exchange_rates table and adds currency columns to itinerary_instances.
    /// Base currency is NPR. Supported: NPR, INR, USD.
    /// </summary>
    [Migration(202604180001)]
    public class AddExchangeRateTables : Migration
    {
        public override void Up()
        {
            // ─── Exchange rate table (stores all rate snapshots) ───
            Execute.Sql(@"
                CREATE TABLE IF NOT EXISTS exchange_rates (
                    id              BIGSERIAL PRIMARY KEY,
                    base_currency   VARCHAR(3)  NOT NULL DEFAULT 'NPR',
                    target_currency VARCHAR(3)  NOT NULL,
                    rate            DECIMAL(18,8) NOT NULL,
                    source          VARCHAR(50) DEFAULT 'manual',
                    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                    created_by      VARCHAR(100)
                );

                CREATE INDEX IF NOT EXISTS idx_exchange_rates_target
                    ON exchange_rates (target_currency, created_at DESC);
            ");

            // ─── Active/latest rates view (quick lookup) ───
            Execute.Sql(@"
                CREATE TABLE IF NOT EXISTS exchange_rates_current (
                    currency_code   VARCHAR(3) PRIMARY KEY,
                    rate            DECIMAL(18,8) NOT NULL,
                    last_updated    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                    updated_by      VARCHAR(100)
                );

                -- Seed default rates
                INSERT INTO exchange_rates_current (currency_code, rate, last_updated)
                VALUES
                    ('NPR', 1.0, NOW()),
                    ('INR', 0.625, NOW()),
                    ('USD', 0.0075, NOW())
                ON CONFLICT (currency_code) DO NOTHING;
            ");

            // ─── Add currency tracking columns to booking instances ───
            Execute.Sql(@"
                ALTER TABLE itinerary_instances
                    ADD COLUMN IF NOT EXISTS selected_currency  VARCHAR(3) DEFAULT 'NPR',
                    ADD COLUMN IF NOT EXISTS exchange_rate_used DECIMAL(18,8) DEFAULT 1.0,
                    ADD COLUMN IF NOT EXISTS base_price_npr     DECIMAL(18,2);
            ");

            // ─── Add currency tracking columns to proposals ───
            Execute.Sql(@"
                ALTER TABLE itinerary_proposals
                    ADD COLUMN IF NOT EXISTS selected_currency  VARCHAR(3) DEFAULT 'NPR',
                    ADD COLUMN IF NOT EXISTS exchange_rate_used DECIMAL(18,8) DEFAULT 1.0,
                    ADD COLUMN IF NOT EXISTS base_price_npr     DECIMAL(18,2);
            ");
        }

        public override void Down()
        {
            Execute.Sql("ALTER TABLE itinerary_proposals DROP COLUMN IF EXISTS base_price_npr;");
            Execute.Sql("ALTER TABLE itinerary_proposals DROP COLUMN IF EXISTS exchange_rate_used;");
            Execute.Sql("ALTER TABLE itinerary_proposals DROP COLUMN IF EXISTS selected_currency;");

            Execute.Sql("ALTER TABLE itinerary_instances DROP COLUMN IF EXISTS base_price_npr;");
            Execute.Sql("ALTER TABLE itinerary_instances DROP COLUMN IF EXISTS exchange_rate_used;");
            Execute.Sql("ALTER TABLE itinerary_instances DROP COLUMN IF EXISTS selected_currency;");

            Execute.Sql("DROP TABLE IF EXISTS exchange_rates_current;");
            Execute.Sql("DROP TABLE IF EXISTS exchange_rates;");
        }
    }
}
