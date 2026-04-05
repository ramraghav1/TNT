using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608310006)]
    public class AddTenantIdToUserInformation : Migration
    {
        public override void Up()
        {
            Alter.Table("userinformation")
                .AddColumn("tenant_id").AsInt64().Nullable();

            Create.ForeignKey("fk_userinformation_tenant")
                .FromTable("userinformation").ForeignColumn("tenant_id")
                .ToTable("saas_tenants").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.ForeignKey("fk_userinformation_tenant").OnTable("userinformation");
            Delete.Column("tenant_id").FromTable("userinformation");
        }
    }
}
