using FluentMigrator;
using BCrypt.Net;

namespace DbDeployment.Migrations;

[Migration(202507290002)]
public class SeedManagerUserInformation : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
        INSERT INTO userinformation (
            userfullname,
            address,
            emailaddress,
            mobilenumber,
            createdby,
            updatedby,
            createdat,
            updatedat
        )
        VALUES (
            'manager admin',
            '123 Main Street',
            'john.doe@example.com',
            '9845545454',
            1,
            1,
            CURRENT_TIMESTAMP,
            CURRENT_TIMESTAMP
        );
    ");
    }

    public override void Down()
    {
        Execute.Sql("DELETE FROM userinformation WHERE userid = '1';");
    }
}
