using PAM.Core;
using PAM.Core.Sessions;
using PAM.DataLayer;
using PAM.DataLayer.Repositories;
using PAM.Domain.Contracts;
using PAM.Metrics;
using PAM.Services;
using PAM.Services.Account;
using PAM.Services.Dashboard;
using Prometheus.Client.AspNetCore;
using Prometheus.Client.DependencyInjection;

namespace PAM;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
        => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<DbOptions>(_configuration.GetSection(nameof(DbOptions)));
        services.AddSingleton<ConnectionFactory>();

        services
            .AddSingleton<IAccountRepository, AccountRepository>()
            .AddSingleton<IResourceRepository, ResourceRepository>()
            .AddSingleton<IPermissionRepository, PermissionRepository>()
            .AddSingleton<ILogRepository, LogRepository>()
            .AddSingleton<ISessionRepository, SessionRepository>();

        services
            .AddSingleton<ActivityLogger>()
            .AddSingleton<AccountMetrics>()
            .AddSingleton<SessionMetrics>()
            .AddHostedService<MetricsJob>();

        services
            .AddSingleton<IAccountService, AccountService>()
            .Decorate<IAccountService, AccountMetricsDecorator>()
            .Decorate<IAccountService, AccountLoggingDecorator>();

        services
            .AddSingleton<IDashboardService, DashboardService>()
            .Decorate<IDashboardService, DashboardLoggingDecorator>();

        services
            .AddSingleton<MonitoringService>()
            .AddSingleton<SessionManager>();

        services.AddControllersWithViews();
        services.AddHttpContextAccessor();
        services.AddSession();
        services.AddMetricFactory();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        => app
            .UseRouting()
            .UseAuthorization()
            .UseSession()
            .UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(30) })
            .UsePrometheusServer(o => o.MapPath = "/metrics")
            .UseEndpoints(e =>
            {
                e.MapControllers();
            });
}
