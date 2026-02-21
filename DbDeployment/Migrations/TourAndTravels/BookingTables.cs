using FluentMigrator;
using System;

namespace Migrations
{
    [Migration(20260221_001)]
    public class BookingTables : Migration
    {
        public override void Up()
        {
            // ==========================
            // Booking Master Table
            // ==========================
            Create.Table("booking")
                .WithColumn("instanceid").AsInt64().PrimaryKey().Identity()
                .WithColumn("itineraryid").AsInt64().NotNullable()
                .WithColumn("startdate").AsDateTime().NotNullable()
                .WithColumn("enddate").AsDateTime().NotNullable()
                .WithColumn("status").AsString(50).NotNullable().WithDefaultValue("Draft")
                .WithColumn("totalamount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("amountpaid").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("balanceamount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("specialrequests").AsString(int.MaxValue).Nullable()
                .WithColumn("createdat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updatedat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("fk_booking_itinerary")
                .FromTable("booking").ForeignColumn("itineraryid")
                .ToTable("itineraries").PrimaryColumn("id");

            // ==========================
            // Booking Travelers Table
            // ==========================
            Create.Table("bookingtraveler")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("bookingid").AsInt64().NotNullable()
                .WithColumn("fullname").AsString(200).NotNullable()
                .WithColumn("contactnumber").AsString(50).Nullable()
                .WithColumn("email").AsString(200).Nullable()
                .WithColumn("nationality").AsString(100).Nullable()
                .WithColumn("adults").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("children").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("seniors").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("createdat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updatedat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("fk_bookingtraveler_booking")
                .FromTable("bookingtraveler").ForeignColumn("bookingid")
                .ToTable("booking").PrimaryColumn("instanceid");

            // ==========================
            // Booking Days Table
            // ==========================
            Create.Table("bookingday")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("bookingid").AsInt64().NotNullable()
                .WithColumn("instancedayid").AsInt64().NotNullable() // reference to template day
                .WithColumn("title").AsString(200).Nullable()
                .WithColumn("location").AsString(200).Nullable()
                .WithColumn("accommodation").AsString(200).Nullable()
                .WithColumn("transport").AsString(100).Nullable()
                .WithColumn("breakfastincluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("lunchincluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("dinnerincluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("activities").AsString(int.MaxValue).Nullable() // comma separated list
                .WithColumn("createdat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updatedat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("fk_bookingday_booking")
                .FromTable("bookingday").ForeignColumn("bookingid")
                .ToTable("booking").PrimaryColumn("instanceid");

            Create.ForeignKey("fk_bookingday_itineraryday")
                .FromTable("bookingday").ForeignColumn("instancedayid")
                .ToTable("itinerarydays").PrimaryColumn("id");

            // ==========================
            // Booking Payments Table
            // ==========================
            Create.Table("bookingpayment")
                .WithColumn("paymentid").AsInt64().PrimaryKey().Identity()
                .WithColumn("bookingid").AsInt64().NotNullable()
                .WithColumn("amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("paymentdate").AsDateTime().NotNullable()
                .WithColumn("paymentmethod").AsString(50).Nullable()
                .WithColumn("createdat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("fk_bookingpayment_booking")
                .FromTable("bookingpayment").ForeignColumn("bookingid")
                .ToTable("booking").PrimaryColumn("instanceid");
        }

        public override void Down()
        {
            Delete.Table("bookingpayment");
            Delete.Table("bookingday");
            Delete.Table("bookingtraveler");
            Delete.Table("booking");
        }
    }
}