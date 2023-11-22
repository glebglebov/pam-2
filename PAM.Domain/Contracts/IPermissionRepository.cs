using PAM.Domain.Models;

namespace PAM.Domain.Contracts;

public interface IPermissionRepository
{
    Task<int> GetPermissionCount(int userId, int resourceId, CancellationToken cancellationToken);
    Task<IEnumerable<TargetResource>> GetResourcesByUserId(int userId, CancellationToken cancellationToken);
    Task Create(int userId, int resourceId, CancellationToken cancellationToken);
    Task Delete(int userId, int resourceId, CancellationToken cancellationToken);
}
