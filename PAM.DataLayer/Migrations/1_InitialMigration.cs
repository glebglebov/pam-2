using FluentMigrator;

namespace PAM.DataLayer.Migrations;

[Migration(1)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        var resTable = Create.Table("target_resources");
        var resCredTable = Create.Table("target_resource_credentials");

        var usersTable = Create.Table("users");
        var permTable = Create.Table("user_permissions");

        var sessionsTable = Create.Table("sessions");

        var logTable = Create.Table("logs");
        
        // target_resources
        resTable.WithColumn("id")
            .AsInt32()
            .PrimaryKey()
            .Unique();
        resTable.WithColumn("address")
            .AsString(256)
            .NotNullable();
        resTable.WithColumn("name")
            .AsString(256)
            .NotNullable();
        
        // target_resource_credentials
        resCredTable.WithColumn("id")
            .AsInt32()
            .PrimaryKey()
            .Unique();
        resCredTable.WithColumn("resource_id")
            .AsInt32()
            .NotNullable();
        resCredTable.WithColumn("login")
            .AsString(256)
            .NotNullable();
        resCredTable.WithColumn("password")
            .AsString(256)
            .NotNullable();
        
        // users
        usersTable.WithColumn("id")
            .AsInt32()
            .PrimaryKey()
            .Unique();
        usersTable.WithColumn("login")
            .AsString(256)
            .NotNullable()
            .Unique();
        usersTable.WithColumn("password_hash")
            .AsString(256)
            .NotNullable();
        usersTable.WithColumn("level")
            .AsInt32()
            .WithDefaultValue(1)
            .NotNullable();
        
        // user_permissions
        permTable.WithColumn("user_id")
            .AsInt32()
            .NotNullable();
        permTable.WithColumn("resource_id")
            .AsInt32()
            .NotNullable();
        
        // sessions
        sessionsTable.WithColumn("guid")
            .AsGuid()
            .PrimaryKey()
            .Unique();
        sessionsTable.WithColumn("user_id")
            .AsInt32()
            .NotNullable();
        sessionsTable.WithColumn("user_ip")
            .AsString()
            .NotNullable();
        sessionsTable.WithColumn("resource_id")
            .AsInt32()
            .NotNullable();
        sessionsTable.WithColumn("begin_timestamp")
            .AsDateTime()
            .NotNullable();
        sessionsTable.WithColumn("end_timestamp")
            .AsDateTime()
            .Nullable();
        
        // logs
        logTable.WithColumn("guid")
            .AsGuid()
            .PrimaryKey()
            .Unique();
        logTable.WithColumn("title")
            .AsString()
            .NotNullable();
        logTable.WithColumn("data")
            .AsString()
            .NotNullable();
        logTable.WithColumn("timestamp")
            .AsDateTime()
            .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("target_resources");
        Delete.Table("target_resource_credentials");
        Delete.Table("users");
        Delete.Table("user_permissions");
        Delete.Table("sessions");
        Delete.Table("logs");
    }
}
