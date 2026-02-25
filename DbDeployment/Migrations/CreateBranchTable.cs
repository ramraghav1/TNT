using System;
using FluentMigrator;

namespace DbDeployment.Migrations
{
	
        [Migration(202508100004)]
        public class CreateBranchTable : Migration
        {
            public override void Up()
            {
                Create.Table("branch")
                    .WithColumn("branch_id").AsInt64().PrimaryKey().Identity()
                    .WithColumn("organization_id").AsInt64().NotNullable()
                    .WithColumn("branch_name").AsString(255).NotNullable()
                    .WithColumn("address").AsString(500).Nullable()
                    .WithColumn("state").AsString(100).Nullable()
                    .WithColumn("district").AsString(100).Nullable()
                    .WithColumn("locallevel").AsString(100).Nullable()
                    .WithColumn("ward_number").AsInt32().Nullable()
                    .WithColumn("zipcode").AsString(20).Nullable()
                    .WithColumn("status").AsString(20).WithDefaultValue("active")
                    .WithColumn("contact_person").AsString(255).Nullable()
                    .WithColumn("contact_email").AsString(255).Nullable()
                    .WithColumn("contact_phone").AsString(50).Nullable()
                    .WithColumn("created_at").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                    .WithColumn("created_by").AsInt64().NotNullable()
                    .WithColumn("updated_at").AsDateTime().Nullable()
                    .WithColumn("updated_by").AsInt64().Nullable();

                Create.ForeignKey("fk_branch_organization")
                    .FromTable("branch").ForeignColumn("organization_id")
                    .ToTable("organization").PrimaryColumn("organization_id")
                    .OnDeleteOrUpdate(System.Data.Rule.None);
            }

            public override void Down()
            {
                Delete.Table("branch");
            }
        }
    }


