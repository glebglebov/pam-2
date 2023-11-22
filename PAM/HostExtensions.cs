using FluentMigrator.Runner;
using Microsoft.Extensions.Options;
using PAM.DataLayer;
using PAM.DataLayer.Migrations;

namespace PAM;

public static class HostExtensions
{
    public static void Migrate(this IHost host)
    {
        var options = host.Services.GetRequiredService<IOptions<DbOptions>>();
        var connectionString = options.Value.ConnectionString;

        var provider = CreateServices(connectionString);
        using var scope = provider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
    
    private static IServiceProvider CreateServices(string connectionString)
        => new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(InitialMigration).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
}
