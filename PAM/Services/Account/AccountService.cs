using OtpNet;
using PAM.Core.Auth;
using PAM.Domain.Contracts;

namespace PAM.Services.Account;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly IHttpContextAccessor _accessor;

    public AccountService(IAccountRepository repository, IHttpContextAccessor accessor)
    {
        _repository = repository;
        _accessor = accessor;
    }

    public async Task<bool> TryLogin(string login, string password, CancellationToken cancellationToken)
    {
        var correctHash = await _repository.GetPasswordHashByLogin(login, cancellationToken);
        if (correctHash == null)
            return false;

        var inputHash = HashHelpers.GetHash(password);
        if (inputHash != correctHash)
            return false;

        var userId = await _repository.GetUserIdByLogin(login, cancellationToken);
        if (userId == null)
            return false;

        var user = await _repository.GetUserById(userId.Value, cancellationToken);
        if (user == null)
            return false;

        if (!user.Is2FaEnabled)
        {
            _accessor.HttpContext?.SetIsVerifiedToTrue(userId.Value);
        }
        else
        {
            _accessor.HttpContext?.SetUserId(userId.Value);
        }

        return true;
    }

    public async Task<bool> TryVerifyToken(int userId, string token, CancellationToken cancellationToken)
    {
        var secretKey = await _repository.GetUserSecretKey(userId, cancellationToken);
        if (secretKey == null)
            return false;

        var totp = new Totp(Base32Encoding.ToBytes(secretKey));
        var isValid = totp.VerifyTotp(token, out _);
        if (!isValid)
            return false;

        _accessor.HttpContext?.SetIsVerifiedToTrue(userId);
        return true;
    }
}
