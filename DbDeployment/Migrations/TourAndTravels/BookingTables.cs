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
            Create.Table("Booking")
                .WithColumn("InstanceId").AsInt64().PrimaryKey().Identity()
                .WithColumn("ItineraryId").AsInt64().NotNullable()
                .WithColumn("StartDate").AsDateTime().NotNullable()
                .WithColumn("EndDate").AsDateTime().NotNullable()
                .WithColumn("Status").AsString(50).NotNullable().WithDefaultValue("Draft")
                .WithColumn("TotalAmount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("AmountPaid").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("BalanceAmount").AsDecimal(18, 2).NotNullable().WithDefaultValue(0)
                .WithColumn("SpecialRequests").AsString(int.MaxValue).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("UpdatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_Booking_Itinerary")
                .FromTable("Booking").ForeignColumn("ItineraryId")
                .ToTable("Itineraries").PrimaryColumn("Id");

            // ==========================
            // Booking Travelers Table
            // ==========================
            Create.Table("BookingTraveler")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingId").AsInt64().NotNullable()
                .WithColumn("FullName").AsString(200).NotNullable()
                .WithColumn("ContactNumber").AsString(50).Nullable()
                .WithColumn("Email").AsString(200).Nullable()
                .WithColumn("Nationality").AsString(100).Nullable()
                .WithColumn("Adults").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("Children").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("Seniors").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("UpdatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_BookingTraveler_Booking")
                .FromTable("BookingTraveler").ForeignColumn("BookingId")
                .ToTable("Booking").PrimaryColumn("InstanceId");

            // ==========================
            // Booking Days Table
            // ==========================
            Create.Table("BookingDay")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingId").AsInt64().NotNullable()
                .WithColumn("InstanceDayId").AsInt64().NotNullable() // reference to template day
                .WithColumn("Title").AsString(200).Nullable()
                .WithColumn("Location").AsString(200).Nullable()
                .WithColumn("Accommodation").AsString(200).Nullable()
                .WithColumn("Transport").AsString(100).Nullable()
                .WithColumn("BreakfastIncluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("LunchIncluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("DinnerIncluded").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("Activities").AsString(int.MaxValue).Nullable() // comma separated list
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("UpdatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_BookingDay_Booking")
                .FromTable("BookingDay").ForeignColumn("BookingId")
                .ToTable("Booking").PrimaryColumn("InstanceId");

            Create.ForeignKey("FK_BookingDay_ItineraryDay")
                .FromTable("BookingDay").ForeignColumn("InstanceDayId")
                .ToTable("ItineraryDays").PrimaryColumn("Id");

            // ==========================
            // Booking Payments Table
            // ==========================
            Create.Table("BookingPayment")
                .WithColumn("PaymentId").AsInt64().PrimaryKey().Identity()
                .WithColumn("BookingId").AsInt64().NotNullable()
                .WithColumn("Amount").AsDecimal(18, 2).NotNullable()
                .WithColumn("PaymentDate").AsDateTime().NotNullable()
                .WithColumn("PaymentMethod").AsString(50).Nullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_BookingPayment_Booking")
                .FromTable("BookingPayment").ForeignColumn("BookingId")
                .ToTable("Booking").PrimaryColumn("InstanceId");
        }

        public override void Down()
        {
            Delete.Table("BookingPayment");
            Delete.Table("BookingDay");
            Delete.Table("BookingTraveler");
            Delete.Table("Booking");
        }
    }
}