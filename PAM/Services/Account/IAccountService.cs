namespace PAM.Services.Account;

public interface IAccountService
{
    Task<bool> TryLogin(string login, string password, CancellationToken cancellationToken);
    Task<bool> TryVerifyToken(int userId, string token, CancellationToken cancellationToken);
}
