using System.Data;
using Dapper;

namespace PAM.DataLayer.Helpers;

public static class CommandHelpers
{
    private const int DefaultTimeout = 10;
    
    public static CommandDefinition CreateCommand(
        string query,
        object? parameters = null,
        CancellationToken cancellationToken = default)
        => new(
            commandText: query,
            parameters: parameters,
            commandType: CommandType.Text,
            commandTimeout: DefaultTimeout,
            cancellationToken: cancellationToken);
}
