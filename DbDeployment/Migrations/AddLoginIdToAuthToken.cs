using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608300002)]
    public class AddLoginIdToAuthToken : Migration
    {
        public override void Up()
        {
            Alter.Table("authtoken")
                .AddColumn("login_id").AsInt64().Nullable();

            Create.ForeignKey("fk_authtoken_logindetail")
                .FromTable("authtoken").ForeignColumn("login_id")
                .ToTable("logindetail").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.ForeignKey("fk_authtoken_logindetail").OnTable("authtoken");
            Delete.Column("login_id").FromTable("authtoken");
        }
    }
}
