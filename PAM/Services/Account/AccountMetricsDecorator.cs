using JetBrains.Annotations;
using PAM.Metrics;

namespace PAM.Services.Account;

[UsedImplicitly]
public class AccountMetricsDecorator : IAccountService
{
    private readonly IAccountService _target;
    private readonly AccountMetrics _metrics;

    public AccountMetricsDecorator(IAccountService target, AccountMetrics metrics)
    {
        _target = target;
        _metrics = metrics;
    }

    public async Task<bool> TryLogin(string login, string password, CancellationToken cancellationToken)
    {
        var isSuccess = await _target.TryLogin(login, password, cancellationToken);
        if (isSuccess)
        {
            _metrics.RegisterSuccessLogin();
            return isSuccess;
        }

        _metrics.RegisterFailedLogin(login);
        return isSuccess;
    }

    public async Task<bool> TryVerifyToken(int userId, string token, CancellationToken cancellationToken)
    {
        return await _target.TryVerifyToken(userId, token, cancellationToken);
    }
}
