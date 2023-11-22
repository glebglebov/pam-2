using PAM.Domain.Models;

namespace PAM.Domain.Contracts;

public interface ILogRepository
{
    Task<int> GetLogCount(CancellationToken cancellationToken);
    Task<IEnumerable<Log>> GetLogs(int offset, int limit, CancellationToken cancellationToken);
    Task AddLog(Guid guid, string title, string data, DateTime timestamp, int level);
}
