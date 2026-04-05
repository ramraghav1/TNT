using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608310008)]
    public class CreatePasswordResetTable : Migration
    {
        public override void Up()
        {
            Create.Table("password_reset_tokens")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable()
                    .ForeignKey("fk_password_reset_user", "userinformation", "userid").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("token").AsString(255).NotNullable().Unique()
                .WithColumn("email").AsString(255).NotNullable()
                .WithColumn("expires_at").AsDateTime().NotNullable()
                .WithColumn("is_used").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("used_at").AsDateTime().Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.Index("idx_password_reset_token")
                .OnTable("password_reset_tokens")
                .OnColumn("token").Ascending();

            Create.Index("idx_password_reset_user")
                .OnTable("password_reset_tokens")
                .OnColumn("user_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("password_reset_tokens");
        }
    }
}
