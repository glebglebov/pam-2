using PAM;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(b => b.UseStartup<Startup>())
    .Build();

if (args.Length > 0 && args[0] == "--migrate")
{
    app.Migrate();
    return;
}

app.Run();
