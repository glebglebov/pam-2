using PAM.Domain.Models;
using PAM.PageModels;

namespace PAM.Services.Dashboard;

public interface IDashboardService
{
    Task AddPermission(int userId, int resourceId, CancellationToken cancellationToken);
    Task RevokePermission(int userId, int resourceId, CancellationToken cancellationToken);
    Task<bool> HasAccess(int userId, int resourceId, CancellationToken cancellationToken);
    Task<TargetResource> GetResource(int resourceId, CancellationToken cancellationToken);
    Task<ResourceOverviewPageModel> GetAllResources(int userId, CancellationToken cancellationToken);
    Task CreateResource(string name, string address, CancellationToken cancellationToken);
    Task<UserOverviewPageModel> GetAllUsers(CancellationToken cancellationToken);
    Task<UserPageModel> GetUser(int userId, CancellationToken cancellationToken);
    Task CreateUser(string login, string password, CancellationToken cancellationToken);
    Task Set2FaFlag(int userId, bool isEnabled, CancellationToken cancellationToken);
}
