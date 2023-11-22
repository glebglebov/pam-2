using JetBrains.Annotations;
using PAM.Core;
using PAM.Core.Auth;

// ReSharper disable StringLiteralTypo

namespace PAM.Services.Account;

[UsedImplicitly]
public class AccountLoggingDecorator : IAccountService
{
    private readonly IAccountService _target;
    private readonly IHttpContextAccessor _accessor;
    private readonly ActivityLogger _logger;

    public AccountLoggingDecorator(IAccountService target, IHttpContextAccessor accessor, ActivityLogger logger)
    {
        _target = target;
        _accessor = accessor;
        _logger = logger;
    }

    public async Task<bool> TryLogin(string login, string password, CancellationToken cancellationToken)
    {
        var isSuccess = await _target.TryLogin(login, password, cancellationToken);

        if (!isSuccess)
            return isSuccess;

        var userId = _accessor.HttpContext!.GetUserId();
        var ipAddress = _accessor.HttpContext!.TryGetIp();

        await _logger.LogInfo(
            "Попытка входа в систему",
            new Dictionary<string, string>
            {
                { "Логин пользователя", login },
                { "ID пользователя", userId.ToString() },
                { "IP-адрес", ipAddress ?? "unknown" }
            });

        return isSuccess;
    }

    public Task<bool> TryVerifyToken(int userId, string token, CancellationToken cancellationToken)
        => _target.TryVerifyToken(userId, token, cancellationToken);
}
