using JetBrains.Annotations;
using PAM.Core;
using PAM.Domain.Models;
using PAM.PageModels;

// ReSharper disable StringLiteralTypo

namespace PAM.Services.Dashboard;

[UsedImplicitly]
public class DashboardLoggingDecorator : IDashboardService
{
    private readonly IDashboardService _target;
    private readonly IHttpContextAccessor _accessor;
    private readonly ActivityLogger _logger;

    public DashboardLoggingDecorator(
        IDashboardService target,
        IHttpContextAccessor accessor,
        ActivityLogger logger)
    {
        _target = target;
        _accessor = accessor;
        _logger = logger;
    }

    public async Task AddPermission(int userId, int resourceId, CancellationToken cancellationToken)
    {
        await _target.AddPermission(userId, resourceId, cancellationToken);
        await _logger.LogWarning(
            "Выдан доступ к ресурсу",
            new Dictionary<string, string>
            {
                { "ID пользователя", userId.ToString() },
                { "ID целевого ресурса", resourceId.ToString() }
            });
    }
    
    public async Task RevokePermission(int userId, int resourceId, CancellationToken cancellationToken)
    {
        await _target.RevokePermission(userId, resourceId, cancellationToken);
        await _logger.LogWarning(
            "Отозван доступ к ресурсу",
            new Dictionary<string, string>
            {
                { "ID пользователя", userId.ToString() },
                { "ID целевого ресурса", resourceId.ToString() }
            });
    }
    
    public async Task CreateResource(string name, string address, CancellationToken cancellationToken)
    {
        await _target.CreateResource(name, address, cancellationToken);
    }
    
    public async Task CreateUser(string login, string password, CancellationToken cancellationToken)
    {
        await _target.CreateUser(login, password, cancellationToken);
    }

    public async Task Set2FaFlag(int userId, bool isEnabled, CancellationToken cancellationToken)
    {
        await _target.Set2FaFlag(userId, isEnabled, cancellationToken);
    }

    // not decorated
    
    public Task<bool> HasAccess(int userId, int resourceId, CancellationToken cancellationToken)
        => _target.HasAccess(userId, resourceId, cancellationToken);

    public Task<TargetResource> GetResource(int resourceId, CancellationToken cancellationToken)
        => _target.GetResource(resourceId, cancellationToken);

    public Task<ResourceOverviewPageModel> GetAllResources(int userId, CancellationToken cancellationToken)
        => _target.GetAllResources(userId, cancellationToken);

    public Task<UserOverviewPageModel> GetAllUsers(CancellationToken cancellationToken)
        => _target.GetAllUsers(cancellationToken);

    public Task<UserPageModel> GetUser(int userId, CancellationToken cancellationToken)
        => _target.GetUser(userId, cancellationToken);
}
