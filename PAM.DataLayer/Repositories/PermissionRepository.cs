using Dapper;
using PAM.DataLayer.Helpers;
using PAM.Domain.Contracts;
using PAM.Domain.Models;

namespace PAM.DataLayer.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly ConnectionFactory _factory;

    public PermissionRepository(ConnectionFactory factory)
        => _factory = factory;

    public async Task<int> GetPermissionCount(int userId, int resourceId, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT COUNT(*)
            FROM user_permissions
            WHERE user_id = @UserId AND resource_id = @ResourceId";
        
        var parameters = new
        {
            UserId = userId,
            ResourceId = resourceId
        };
        
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<IEnumerable<TargetResource>> GetResourcesByUserId(int userId, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT tr.id, tr.name, tr.address
            FROM user_permissions up
            LEFT JOIN target_resources tr ON up.resource_id = tr.id
            WHERE up.user_id = @UserId";
        
        var parameters = new { UserId = userId };
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<TargetResource>(command);
    }
    
    public async Task Create(int userId, int resourceId, CancellationToken cancellationToken)
    {
        const string query = "INSERT INTO user_permissions (user_id, resource_id) VALUES(@UserId, @ResourceId)";
        var parameters = new
        {
            UserId = userId,
            ResourceId = resourceId
        };
        
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }

    public async Task Delete(int userId, int resourceId, CancellationToken cancellationToken)
    {
        const string query = "DELETE FROM user_permissions WHERE user_id = @UserId AND resource_id = @ResourceId";
        var parameters = new
        {
            UserId = userId,
            ResourceId = resourceId
        };
        
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }
}
