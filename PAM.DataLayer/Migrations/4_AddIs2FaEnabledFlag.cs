using FluentMigrator;

namespace PAM.DataLayer.Migrations;

[Migration(4)]
public class AddIs2FaEnabledFlag : Migration
{
    public override void Up()
        => Alter.Table("users").AddColumn("is_2fa_enabled")
            .AsBoolean()
            .WithDefaultValue(false)
            .NotNullable();

    public override void Down()
        => Delete
            .Column("is_2fa_enabled")
            .FromTable("users");
}
