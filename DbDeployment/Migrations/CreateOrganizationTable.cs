using System;
using FluentMigrator;

namespace DbDeployment.Migrations
{
    [Migration(202608100002)]
    public class CreateOrganizationTable : Migration
    {
        public override void Up()
        {
            Create.Table("organization")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(255).NotNullable()
                .WithColumn("type").AsString(50).NotNullable()
                .WithColumn("country_iso3").AsString(3).NotNullable()
                .WithColumn("status").AsString(20).WithDefaultValue("ACTIVE")
                .WithColumn("contact_person").AsString(255).Nullable()
                .WithColumn("contact_email").AsString(255).Nullable()
                .WithColumn("contact_phone").AsString(50).Nullable()
                .WithColumn("created_at").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("created_by").AsInt64().NotNullable()
                .WithColumn("updated_at").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_by").AsInt64().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("organization");
        }
    }
}

