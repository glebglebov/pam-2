using PAM.Domain.Models;

namespace PAM.Domain.Contracts;

public interface IAccountRepository
{
    Task<IEnumerable<User>> GetAll(CancellationToken cancellationToken);
    Task<string?> GetPasswordHashByLogin(string login, CancellationToken cancellationToken);
    Task<string?> GetUserSecretKey(int userId, CancellationToken cancellationToken);
    Task<int?> GetUserIdByLogin(string login, CancellationToken cancellationToken);
    User? GetUserById(int id);
    Task<User?> GetUserById(int id, CancellationToken cancellationToken);
    Task Create(string login, string passwordHash, string secretKey, CancellationToken cancellationToken);
    Task Set2FaFlag(int userId, bool isEnabled, CancellationToken cancellationToken);
}
