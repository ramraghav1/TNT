using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608310002)]
    public class AddTargetUserIdToNotifications : Migration
    {
        public override void Up()
        {
            // NULL = broadcast to all users, SET = only for that specific user
            Alter.Table("notifications")
                .AddColumn("target_user_id").AsInt32().Nullable();

            // Add unique constraint for upsert ON CONFLICT support
            Create.UniqueConstraint("uq_user_notifications_notification_user")
                .OnTable("user_notifications")
                .Columns("notification_id", "user_id");
        }

        public override void Down()
        {
            Delete.UniqueConstraint("uq_user_notifications_notification_user").FromTable("user_notifications");
            Delete.Column("target_user_id").FromTable("notifications");
        }
    }
}
