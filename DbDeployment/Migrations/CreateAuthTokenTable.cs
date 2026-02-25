using System;
using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202508090001)]
    public class CreateAuthTokenTable: Migration
    {

        public override void Up()
        {
            Create.Table("authtoken")
                .WithColumn("tokenid").AsInt32().PrimaryKey().Identity()
                .WithColumn("userid").AsInt32().NotNullable()
                .WithColumn("refreshtoken").AsString(int.MaxValue).NotNullable()
                .WithColumn("accesstoken").AsString(int.MaxValue).Nullable()
                .WithColumn("issuedat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("expiresat").AsDateTime().NotNullable()
                .WithColumn("revoked").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("revokedat").AsDateTime().Nullable()
                .WithColumn("ipaddress").AsString(45).Nullable()
                .WithColumn("useragent").AsString(int.MaxValue).Nullable()
                .WithColumn("createdat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("updatedat").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            //Delete.Table("logindetail");
        }
    }
	
}

