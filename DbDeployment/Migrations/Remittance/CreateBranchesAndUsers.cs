using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(2026022403)]
    public class CreateBranchesAndUsers : Migration
    {
        public override void Up()
        {
            // 1. Drop old branch and organization tables using CASCADE
            Execute.Sql("DROP TABLE IF EXISTS branch CASCADE;");
            Execute.Sql("DROP TABLE IF EXISTS organization CASCADE;");

            // 3. Add agent_type column to existing agents table
            Alter.Table("agents")
                .AddColumn("agent_type").AsString(50).Nullable()
                .AddColumn("address").AsString(500).Nullable()
                .AddColumn("updated_at").AsDateTime().Nullable()
                .AddColumn("updated_by").AsInt64().Nullable();

            // 4. Create branches table referencing agents
            Create.Table("branches")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("agent_id").AsInt64().NotNullable()
                .ForeignKey("fk_branches_agent", "agents", "id")
                .WithColumn("branch_name").AsString(255).NotNullable()
                .WithColumn("branch_code").AsString(50).Nullable()
                .WithColumn("address").AsString(500).Nullable()
                .WithColumn("state").AsString(100).Nullable()
                .WithColumn("district").AsString(100).Nullable()
                .WithColumn("locallevel").AsString(100).Nullable()
                .WithColumn("ward_number").AsInt32().Nullable()
                .WithColumn("zipcode").AsString(20).Nullable()
                .WithColumn("contact_person").AsString(255).Nullable()
                .WithColumn("contact_email").AsString(255).Nullable()
                .WithColumn("contact_phone").AsString(50).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("ix_branches_agent_id")
                .OnTable("branches")
                .OnColumn("agent_id").Ascending();

            // 5. Create branch_users table referencing branches
            Create.Table("branch_users")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("branch_id").AsInt64().NotNullable()
                .ForeignKey("fk_branch_users_branch", "branches", "id")
                .WithColumn("full_name").AsString(255).NotNullable()
                .WithColumn("email").AsString(255).Nullable()
                .WithColumn("phone").AsString(50).Nullable()
                .WithColumn("role").AsString(100).Nullable()
                .WithColumn("username").AsString(100).Nullable()
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("updated_at").AsDateTime().Nullable();

            Create.Index("ix_branch_users_branch_id")
                .OnTable("branch_users")
                .OnColumn("branch_id").Ascending();
        }

        public override void Down()
        {
            Delete.Table("branch_users");
            Delete.Table("branches");

            Delete.Column("agent_type").FromTable("agents");
            Delete.Column("address").FromTable("agents");
            Delete.Column("updated_at").FromTable("agents");
            Delete.Column("updated_by").FromTable("agents");
        }
    }
}
