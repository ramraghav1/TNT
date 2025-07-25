using FluentMigrator;
using BCrypt.Net;

namespace DbDeployment.Migrations;

[Migration(202507240002)]
public class SeedManagerUser : Migration
{
    public override void Up()
    {
        // Create bcrypt hash
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("manager123"); // replace with your actual password

        Execute.Sql($@"
            INSERT INTO logindetail (userid,username, password, createdat)
            VALUES (1,'manager', '{passwordHash}', CURRENT_TIMESTAMP);
        ");
    }

    public override void Down()
    {
        Execute.Sql("DELETE FROM LoginDetail WHERE username = 'manager';");
    }
}
