using PAM.Domain.Models;

namespace PAM.Domain.Contracts;

public interface ISessionRepository
{
    Task<IEnumerable<Session>> GetAll(CancellationToken cancellationToken);
    Task<int> GetActiveSessionCount(CancellationToken cancellationToken);
    Task CreateSession(Guid guid, int userId, string ip, int resId, DateTime timestamp, CancellationToken ctx);
    Task UpdateSession(Guid guid, DateTime timestamp);
}
