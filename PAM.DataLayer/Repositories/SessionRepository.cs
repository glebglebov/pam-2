using Dapper;
using PAM.DataLayer.Helpers;
using PAM.Domain.Contracts;
using PAM.Domain.Models;

namespace PAM.DataLayer.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ConnectionFactory _factory;

    public SessionRepository(ConnectionFactory factory)
        => _factory = factory;

    public async Task<IEnumerable<Session>> GetAll(CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT s.guid, s.user_id, s.user_ip, tr.name AS resource_name, s.begin_timestamp, s.end_timestamp
            FROM sessions s
            LEFT JOIN target_resources tr ON tr.id = s.resource_id
            ORDER BY s.begin_timestamp DESC";

        var command = CommandHelpers.CreateCommand(query, cancellationToken: cancellationToken);
        await using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<Session>(command);
    }

    public async Task<int> GetActiveSessionCount(CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT COUNT(*)
            FROM sessions
            WHERE end_timestamp IS NULL";

        var command = CommandHelpers.CreateCommand(query, cancellationToken: cancellationToken);
        await using var connection = _factory.CreateConnection();
        return await connection.QuerySingleAsync<int>(command);
    }

    public async Task CreateSession(
        Guid guid,
        int userId,
        string ip,
        int resourceId,
        DateTime timestamp,
        CancellationToken cancellationToken)
    {
        const string query = @"
            INSERT INTO sessions (guid, user_id, user_ip, resource_id, begin_timestamp)
            VALUES (@Guid, @UserId, @UserIp, @ResourceId, @Begin)";

        var parameters = new
        {
            Guid = guid,
            UserId = userId,
            UserIp = ip,
            ResourceId = resourceId,
            Begin = timestamp
        };

        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }

    public async Task UpdateSession(Guid guid, DateTime timestamp)
    {
        const string query = @"UPDATE sessions SET end_timestamp = @End WHERE guid = @Guid";

        var parameters = new
        {
            Guid = guid,
            End = timestamp
        };

        var command = CommandHelpers.CreateCommand(query, parameters);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }
}
