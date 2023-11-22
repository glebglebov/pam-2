using PAM.Domain.Contracts;
using PAM.PageModels;

namespace PAM.Services;

public class MonitoringService
{
    private const int LogsPerPage = 10;

    private readonly ILogRepository _logRepository;
    private readonly ISessionRepository _sessionRepository;

    public MonitoringService(ILogRepository sessionRepository, ISessionRepository sessionRepository1)
    {
        _logRepository = sessionRepository;
        _sessionRepository = sessionRepository1;
    }

    public async Task<LogsPageModel> GetLogsPage(int page, CancellationToken cancellationToken)
    {
        var logCount = await _logRepository.GetLogCount(cancellationToken);
        var pageCount = (int)Math.Ceiling((double)logCount / LogsPerPage);

        var offset = (page - 1) * LogsPerPage;
        var logs = (await _logRepository.GetLogs(offset, LogsPerPage, cancellationToken)).ToArray();

        return new LogsPageModel
        {
            CurrentPage = page,
            PageCount = pageCount,
            Logs = logs
        };
    }

    public async Task<SessionsPageModel> GetSessionsPage(CancellationToken cancellationToken)
    {
        var sessions = (await _sessionRepository.GetAll(cancellationToken)).ToArray();
        return new SessionsPageModel { Sessions = sessions };
    }
}
