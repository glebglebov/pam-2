using PAM.Domain.Contracts;

namespace PAM.Metrics;

public class MetricsJob : BackgroundService
{
    private const int JobIntervalSeconds = 10;

    private readonly ISessionRepository _sessionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly SessionMetrics _sessionMetrics;
    private readonly AccountMetrics _accountMetrics;

    public MetricsJob(
        ISessionRepository sessionRepository,
        IAccountRepository accountRepository,
        SessionMetrics sessionMetrics,
        AccountMetrics accountMetrics)
    {
        _sessionRepository = sessionRepository;
        _accountRepository = accountRepository;
        _sessionMetrics = sessionMetrics;
        _accountMetrics = accountMetrics;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var timeSpan = TimeSpan.FromSeconds(JobIntervalSeconds);
        while (!cancellationToken.IsCancellationRequested)
        {
            await ExecuteJob(cancellationToken);
            await Task.Delay(timeSpan, cancellationToken);
        }
    }

    private async Task ExecuteJob(CancellationToken cancellationToken)
    {
        var sessionCount = await _sessionRepository.GetActiveSessionCount(cancellationToken);
        _sessionMetrics.RegisterActiveSessions(sessionCount);

        var users = (await _accountRepository.GetAll(cancellationToken)).ToArray();
        _accountMetrics.RegisterUsersWithEnabled2Fa(users.Count(u => u.Is2FaEnabled));
        _accountMetrics.RegisterUsersWithDisabled2Fa(users.Count(u => !u.Is2FaEnabled));
    }
}
