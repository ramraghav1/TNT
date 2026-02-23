using FluentMigrator;
using System;

namespace Migrations
{
    /// <summary>
    /// DEPRECATED: Booking is now unified with itineraryinstances.
    /// Old booking/bookingtraveler/bookingday/bookingpayment tables are no longer used.
    /// Booking data lives in: itineraryinstances, itineraryinstancedays,
    /// itineraryinstancedayactivities, travelers, payments, itineraryapprovals.
    /// This migration is kept as a no-op for migration history compatibility.
    /// </summary>
    [Migration(20260221_001)]
    public class BookingTables : Migration
    {
        public override void Up()
        {
            // No-op: booking is now managed through itineraryinstances
        }

        public override void Down()
        {
            // No-op
        }
    }
}