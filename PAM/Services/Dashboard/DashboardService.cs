using PAM.Domain.Contracts;
using PAM.Domain.Exceptions;
using PAM.Domain.Models;
using PAM.PageModels;
using PAM.TOTP;

namespace PAM.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IResourceRepository _resourceRepository;

    public DashboardService(
        IAccountRepository accountRepository,
        IPermissionRepository permissionRepository,
        IResourceRepository resourceRepository)
    {
        _accountRepository = accountRepository;
        _permissionRepository = permissionRepository;
        _resourceRepository = resourceRepository;
    }
    
    public async Task AddPermission(int userId, int resourceId, CancellationToken cancellationToken)
        => await _permissionRepository.Create(userId, resourceId, cancellationToken);
    
    public async Task RevokePermission(int userId, int resourceId, CancellationToken cancellationToken)
        => await _permissionRepository.Delete(userId, resourceId, cancellationToken);
    
    public async Task<bool> HasAccess(int userId, int resourceId, CancellationToken cancellationToken)
    {
        var count = await _permissionRepository.GetPermissionCount(userId, resourceId, cancellationToken);
        return count > 0;
    }
    
    public async Task<TargetResource> GetResource(int resourceId, CancellationToken cancellationToken)
    {
        var resource = await _resourceRepository.GetById(resourceId, cancellationToken);
        if (resource == null)
            throw new NotFoundException();

        return resource;
    }

    public async Task<ResourceOverviewPageModel> GetAllResources(int userId, CancellationToken cancellationToken)
    {
        var user = await _accountRepository.GetUserById(userId, cancellationToken);
        if (user == null)
            throw new Exception();
        
        var resources = (await _resourceRepository.GetAll(cancellationToken)).ToArray();
        
        return new ResourceOverviewPageModel
        {
            CanCreateNew = user.Level > 50,
            Resources = resources
        };
    }
    
    public async Task CreateResource(string name, string address, CancellationToken cancellationToken)
        => await _resourceRepository.Create(name, address, cancellationToken);

    public async Task<UserOverviewPageModel> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = (await _accountRepository.GetAll(cancellationToken)).ToArray();
        return new UserOverviewPageModel { Users = users };
    }

    public async Task<UserPageModel> GetUser(int userId, CancellationToken cancellationToken)
    {
        var user = _accountRepository.GetUserById(userId);
        if (user == null)
            throw new Exception();

        var permissions = (await _permissionRepository.GetResourcesByUserId(user.Id, cancellationToken)).ToArray();
        var resources = (await _resourceRepository.GetAll(cancellationToken)).ToArray();

        var availableResources = resources
            .Where(x => !permissions.Contains(x))
            .ToArray();

        var base64QrCode = TotpHelpers.GetTotpQrCodeAsBase64(user.SecretKey, user.Login);
        return new UserPageModel
        {
            User = user,
            ResourcePermissions = permissions,
            AvailableResources = availableResources,
            Base64QrCode = base64QrCode
        };
    }
    
    public async Task CreateUser(string login, string password, CancellationToken cancellationToken)
    {
        var passwordHash = HashHelpers.GetHash(password);
        var userId = await _accountRepository.GetUserIdByLogin(login, cancellationToken);
        
        if (userId != null)
            throw new Exception();

        await _accountRepository.Create(
            login,
            passwordHash,
            TotpKeyGenerator.GenerateSecretKey(),
            cancellationToken);
    }

    public async Task Set2FaFlag(int userId, bool isEnabled, CancellationToken cancellationToken)
        => await _accountRepository.Set2FaFlag(userId, isEnabled, cancellationToken);
}
