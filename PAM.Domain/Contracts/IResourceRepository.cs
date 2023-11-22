using PAM.Domain.Models;

namespace PAM.Domain.Contracts;

public interface IResourceRepository
{
    Task<IEnumerable<TargetResource>> GetAll(CancellationToken cancellationToken);
    Task<TargetResource?> GetById(int id, CancellationToken cancellationToken);
    Task Create(string name, string address, CancellationToken cancellationToken);
    Task<ResourceCredentials?> GetCredentials(int resourceId, CancellationToken cancellationToken);
}
