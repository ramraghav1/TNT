using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608310003)]
    public class ReplaceTargetUserIdWithIsTargeted : Migration
    {
        public override void Up()
        {
            // Replace single target_user_id with is_targeted flag
            // When is_targeted = true, user_notifications rows define who can see it
            // When is_targeted = false (default), all users can see it
            Delete.Column("target_user_id").FromTable("notifications");
            Alter.Table("notifications")
                .AddColumn("is_targeted").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Column("is_targeted").FromTable("notifications");
            Alter.Table("notifications")
                .AddColumn("target_user_id").AsInt32().Nullable();
        }
    }
}
