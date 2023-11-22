using Prometheus.Client;

namespace PAM.Metrics;

public class AccountMetrics
{
    private readonly IMetricFamily<ICounter> _failedLoginCounter;
    private readonly ICounter _successLoginCounter;
    private readonly IMetricFamily<IGauge> _2FaGauge;

    public AccountMetrics(IMetricFactory factory)
    {
        _failedLoginCounter = factory.CreateCounter(
            "pam_system_auth_failed_counter",
            "Number of failed login attempts",
            labelNames: "login");

        _successLoginCounter = factory.CreateCounter(
            "pam_system_auth_success_counter",
            "Number of successful login attempts");

        _2FaGauge = factory.CreateGauge(
            "pam_system_2fa_enabled_gauge",
            "Number of users by Is2FaEnabled value",
            labelNames: "is_enabled");
    }

    public void RegisterFailedLogin(string login)
        => _failedLoginCounter
            .WithLabels(login)
            .Inc();

    public void RegisterSuccessLogin()
        => _successLoginCounter.Inc();

    public void RegisterUsersWithEnabled2Fa(int count)
        => Register2Fa(true, count);

    public void RegisterUsersWithDisabled2Fa(int count)
        => Register2Fa(false, count);

    private void Register2Fa(bool isEnabled, int count)
        => _2FaGauge
            .WithLabels(isEnabled.ToString())
            .Set(count);
}
