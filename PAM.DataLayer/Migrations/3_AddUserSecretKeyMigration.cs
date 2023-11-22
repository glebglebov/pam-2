using FluentMigrator;

namespace PAM.DataLayer.Migrations;

[Migration(3)]
public class AddUserSecretKeyMigration : Migration
{
    public override void Up()
        => Alter.Table("users").AddColumn("secret_key")
            .AsString(256)
            .WithDefaultValue("not_defined")
            .NotNullable();

    public override void Down()
        => Delete
            .Column("secret_key")
            .FromTable("users");
}
