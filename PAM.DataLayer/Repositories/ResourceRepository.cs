using Dapper;
using PAM.DataLayer.Helpers;
using PAM.Domain.Contracts;
using PAM.Domain.Models;

namespace PAM.DataLayer.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly ConnectionFactory _factory;

    public ResourceRepository(ConnectionFactory factory)
        => _factory = factory;

    public async Task<IEnumerable<TargetResource>> GetAll(CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM target_resources";
        var command = CommandHelpers.CreateCommand(query, cancellationToken: cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<TargetResource>(command);
    }

    public async Task<TargetResource?> GetById(int id, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM target_resources WHERE id = @Id";
        var parameters = new { Id = id };
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TargetResource?>(command);
    }

    public async Task Create(string name, string address, CancellationToken cancellationToken)
    {
        const string query = "INSERT INTO target_resources (address, name) VALUES(@Address, @Name)";
        var parameters = new
        {
            Address = address,
            Name = name
        };
        
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }

    public async Task<ResourceCredentials?> GetCredentials(int resourceId, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT tr.address, trc.login, trc.password
            FROM target_resource_credentials trc
            LEFT JOIN target_resources tr ON trc.resource_id = tr.id
            WHERE tr.id = @ResourceId";
        
        var parameters = new { ResourceId = resourceId };
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ResourceCredentials?>(command);
    }
}
