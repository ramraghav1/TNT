using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202602250004)]
    public class MakeAgentAccountFieldsNullable : Migration
    {
        public override void Up()
        {
            Alter.Table("agent_accounts")
                .AlterColumn("account_name").AsString(200).Nullable();
            Alter.Table("agent_accounts")
                .AlterColumn("account_number").AsString(100).Nullable();
        }

        public override void Down()
        {
            Alter.Table("agent_accounts")
                .AlterColumn("account_name").AsString(200).NotNullable();
            Alter.Table("agent_accounts")
                .AlterColumn("account_number").AsString(100).NotNullable();
        }
    }
}
