using FluentMigrator;

namespace DbDeployment.Migrations.Remittance
{
    [Migration(202602250001)]
    public class CreateAdministrativeDivisionTable : Migration
    {
        public override void Up()
        {
            Create.Table("administrative_divisions")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("country_id").AsInt64().NotNullable()
                    .ForeignKey("fk_admdiv_country", "countries", "id")
                .WithColumn("name").AsString(200).NotNullable()
                .WithColumn("code").AsString(50).Nullable()
                .WithColumn("division_level").AsInt32().NotNullable()
                .WithColumn("parent_id").AsInt64().Nullable()
                    .ForeignKey("fk_admdiv_parent", "administrative_divisions", "id")
                .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("created_at").AsDateTime().NotNullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime);

            // Index on country + level for fast lookups
            Create.Index("ix_admdiv_country_level")
                .OnTable("administrative_divisions")
                .OnColumn("country_id").Ascending()
                .OnColumn("division_level").Ascending();

            // Index on parent for tree traversal
            Create.Index("ix_admdiv_parent_id")
                .OnTable("administrative_divisions")
                .OnColumn("parent_id").Ascending();

            // Unique: same name under same parent in same country
            Create.Index("ix_admdiv_unique_name")
                .OnTable("administrative_divisions")
                .OnColumn("country_id").Ascending()
                .OnColumn("parent_id").Ascending()
                .OnColumn("name").Ascending()
                .WithOptions().Unique();
        }

        public override void Down()
        {
            Delete.Table("administrative_divisions");
        }
    }
}
