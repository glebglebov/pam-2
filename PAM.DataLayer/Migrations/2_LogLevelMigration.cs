using FluentMigrator;

namespace PAM.DataLayer.Migrations;

[Migration(2)]
public class LogLevelMigration : Migration
{
    public override void Up()
        => Alter.Table("logs").AddColumn("level")
            .AsInt32()
            .WithDefaultValue(1)
            .NotNullable();

    public override void Down()
        => Delete
            .Column("level")
            .FromTable("logs");
}
