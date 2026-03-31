using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608310001)]
    public class CreateNotificationTables : Migration
    {
        public override void Up()
        {
            // Shared notification record
            Create.Table("notifications")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("type").AsString(50).NotNullable()        // demo_request, itinerary, booking
                .WithColumn("title").AsString(255).NotNullable()
                .WithColumn("message").AsString(1000).NotNullable()
                .WithColumn("link").AsString(500).Nullable()          // frontend route to navigate to
                .WithColumn("icon").AsString(50).Nullable()           // pi-xxx icon class
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            // Per-user notification status
            Create.Table("user_notifications")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("notification_id").AsInt64().NotNullable()
                .WithColumn("user_id").AsInt32().NotNullable()
                .WithColumn("is_read").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("is_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("read_at").AsDateTime().Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("fk_user_notifications_notification")
                .FromTable("user_notifications").ForeignColumn("notification_id")
                .ToTable("notifications").PrimaryColumn("id");

            Create.Index("ix_user_notifications_user_id")
                .OnTable("user_notifications")
                .OnColumn("user_id").Ascending()
                .OnColumn("is_deleted").Ascending()
                .OnColumn("created_at").Descending();
        }

        public override void Down()
        {
            Delete.Table("user_notifications");
            Delete.Table("notifications");
        }
    }
}
