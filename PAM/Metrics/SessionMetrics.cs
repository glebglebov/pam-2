using Prometheus.Client;

namespace PAM.Metrics;

public class SessionMetrics
{
    private readonly IGauge _activeSessionGauge;

    public SessionMetrics(IMetricFactory factory)
    {
        _activeSessionGauge = factory.CreateGauge(
            "pam_system_sessions_active_gauge",
            "Number of active sessions");
    }

    public void RegisterActiveSessions(int count)
        => _activeSessionGauge.Set(count);
}
