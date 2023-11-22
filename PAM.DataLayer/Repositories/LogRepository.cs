using Dapper;
using PAM.DataLayer.Helpers;
using PAM.Domain.Contracts;
using PAM.Domain.Models;

namespace PAM.DataLayer.Repositories;

public class LogRepository : ILogRepository
{
    private readonly ConnectionFactory _factory;

    public LogRepository(ConnectionFactory factory)
        => _factory = factory;

    public async Task<int> GetLogCount(CancellationToken cancellationToken)
    {
        const string query = "SELECT COUNT(*) FROM logs";
        var command = CommandHelpers.CreateCommand(query, cancellationToken: cancellationToken);
        
        await using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<IEnumerable<Log>> GetLogs(int offset, int limit, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM logs ORDER BY timestamp DESC LIMIT @Count OFFSET @Offset ";
        var parameters = new
        {
            Offset = offset,
            Count = limit
        };
        
        var command = CommandHelpers.CreateCommand(query, parameters, cancellationToken);
        await using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<Log>(command);
    }

    public async Task AddLog(
        Guid guid,
        string title,
        string data,
        DateTime timestamp,
        int level)
    {
        const string query = @"
            INSERT INTO logs (guid, title, data, timestamp, level)
            VALUES(@Guid, @Title, @Data, @Timestamp, @Level)";
        
        var parameters = new
        {
            Guid = guid,
            Title = title,
            Data = data,
            Timestamp = timestamp,
            Level = level
        };
        
        var command = CommandHelpers.CreateCommand(query, parameters);
        await using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(command);
    }
    
    //public async Task<Log[]> GetByPeriod()
}
