using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608290001)]
    public class AddOrgIdToLoginDetail : Migration
    {
        public override void Up()
        {
            Alter.Table("logindetail")
                .AddColumn("org_id").AsInt64().Nullable();

            Create.ForeignKey("fk_logindetail_organization")
                .FromTable("logindetail").ForeignColumn("org_id")
                .ToTable("organization").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.ForeignKey("fk_logindetail_organization").OnTable("logindetail");
            Delete.Column("org_id").FromTable("logindetail");
        }
    }
}
