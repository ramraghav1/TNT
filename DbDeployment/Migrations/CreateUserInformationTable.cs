using FluentMigrator;

namespace Migrations.Migrations;

[Migration(20250729001)]
public class UserInformationTable : Migration
{
    public override void Up()
    {
        Create.Table("userinformation")
            .WithColumn("userid").AsInt32().PrimaryKey().Identity()
            .WithColumn("userfullname").AsString(100).Nullable()
            .WithColumn("address").AsString(255).NotNullable()
            .WithColumn("emailaddress").AsString(255).NotNullable()
            .WithColumn("mobilenumber").AsString(20).Nullable()
            .WithColumn("createdby").AsInt32().Nullable()
            .WithColumn("updatedby").AsInt32().Nullable()
            .WithColumn("createdat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updatedat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
        //Delete.Table("userinformation");
    }
}
