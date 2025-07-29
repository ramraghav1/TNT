using FluentMigrator;

namespace Migrations.Migrations;

[Migration(20250724001)]
public class LoginDetailTable : Migration
{
    public override void Up()
    {
        Create.Table("logindetail")
            .WithColumn("loginid").AsInt32().PrimaryKey().Identity()
            .WithColumn("userid").AsInt32().NotNullable()
            .WithColumn("username").AsString(100).Nullable()
            .WithColumn("password").AsString(255).NotNullable()
            .WithColumn("logintime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("logouttime").AsDateTime().Nullable()
            .WithColumn("ipaddress").AsString(45).Nullable()
            .WithColumn("useragent").AsString(int.MaxValue).Nullable()
            .WithColumn("issuccessful").AsBoolean().WithDefaultValue(true)
            .WithColumn("failurereason").AsString(255).Nullable()
            .WithColumn("location").AsString(255).Nullable()
            .WithColumn("createdat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("updatedat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
        //Delete.Table("logindetail");
    }
}
