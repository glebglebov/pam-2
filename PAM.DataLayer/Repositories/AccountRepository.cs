using Dapper;
using PAM.DataLayer.Helpers;
using PAM.Domain.Contracts;
using PAM.Domain.Models;

namespace PAM.DataLayer.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ConnectionFactory _factory;

    public AccountRepository(ConnectionFactory factory)
        => _factory = factory;

    public async Task<IEnumerable<User>> GetAll(CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM users";
        var command = CommandHelpers.CreateCommand(query, cancellationToken: cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<User>(command);
    }

    public async Task<string?> GetPasswordHashByLogin(string login, CancellationToken cancellationToken)
    {
        const string query = "SELECT password_hash FROM users WHERE login = @Login";
        var parameters = new { Login = login };
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<string?>(command);
    }

    public async Task<string?> GetUserSecretKey(int userId, CancellationToken cancellationToken)
    {
        const string query = "SELECT secret_key FROM users WHERE id = @UserId";
        var parameters = new { UserId = userId };
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);

        await using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<string?>(command);
    }

    public async Task<int?> GetUserIdByLogin(string login, CancellationToken cancellationToken)
    {
        const string query = "SELECT id FROM users WHERE login = @Login";
        var parameters = new { Login = login };
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<int?>(command);
    }

    public User? GetUserById(int id)
    {
        const string query = "SELECT * FROM users WHERE id = @Id";
        var parameters = new { Id = id };
        var command = CommandHelpers.CreateCommand(query, parameters);
        
        using var connection = _factory.CreateConnection();
        return connection.QueryFirstOrDefault<User?>(command);
    }

    public async Task<User?> GetUserById(int id, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM users WHERE id = @Id";
        var parameters = new { Id = id };
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User?>(command);
    }

    public async Task Create(string login, string passwordHash, string secretKey, CancellationToken cancellationToken)
    {
        const string query = "INSERT INTO users (login, password_hash, secret_key) VALUES(@Login, @PasswordHash, @Key)";
        var parameters = new
        {
            Login = login,
            PasswordHash = passwordHash,
            Key = secretKey
        };

        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }

    public async Task Set2FaFlag(int userId, bool isEnabled, CancellationToken cancellationToken)
    {
        const string query = @"UPDATE users SET is_2fa_enabled = @IsEnabled WHERE id = @UserId";
        var parameters = new
        {
            UserId = userId,
            IsEnabled = isEnabled
        };

        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }
}
