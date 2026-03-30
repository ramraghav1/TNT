using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608300001)]
    public class CreateDemoRequestTable : Migration
    {
        public override void Up()
        {
            Create.Table("demo_request")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("full_name").AsString(255).NotNullable()
                .WithColumn("email").AsString(255).NotNullable()
                .WithColumn("phone").AsString(50).Nullable()
                .WithColumn("company_name").AsString(255).Nullable()
                .WithColumn("product_interest").AsString(50).NotNullable()
                .WithColumn("message").AsString(1000).Nullable()
                .WithColumn("status").AsString(20).WithDefaultValue("NEW")
                .WithColumn("created_at").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            Delete.Table("demo_request");
        }
    }
}
