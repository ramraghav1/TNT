using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608310005)]
    public class AddTenantIdToLoginDetail : Migration
    {
        public override void Up()
        {
            Alter.Table("logindetail")
                .AddColumn("tenant_id").AsInt64().Nullable();

            Create.ForeignKey("fk_logindetail_tenant")
                .FromTable("logindetail").ForeignColumn("tenant_id")
                .ToTable("saas_tenants").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.ForeignKey("fk_logindetail_tenant").OnTable("logindetail");
            Delete.Column("tenant_id").FromTable("logindetail");
        }
    }
}
