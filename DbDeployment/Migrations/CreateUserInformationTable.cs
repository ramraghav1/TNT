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
[Migration(20250810_05)]
public class AddBranchIdAndOrgIdToUserInformation : Migration
{
    public override void Up()
    {
        Alter.Table("userinformation")
            .AddColumn("branch_id").AsInt64().Nullable()
            .AddColumn("org_id").AsInt64().Nullable();

        // Optional: Add foreign keys if you want referential integrity enforced
        Create.ForeignKey("fk_userinformation_branch")
            .FromTable("userinformation").ForeignColumn("branch_id")
            .ToTable("branch").PrimaryColumn("branch_id");

        Create.ForeignKey("fk_userinformation_organization")
            .FromTable("userinformation").ForeignColumn("org_id")
            .ToTable("organization").PrimaryColumn("organization_id");
    }

    public override void Down()
    {
        Delete.ForeignKey("fk_userinformation_branch").OnTable("userinformation");
        Delete.ForeignKey("fk_userinformation_organization").OnTable("userinformation");

        Delete.Column("branch_id").FromTable("userinformation");
        Delete.Column("org_id").FromTable("userinformation");
    }
}