using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace PAM.DataLayer;

public class ConnectionFactory
{
    private readonly string _connectionString;

    public ConnectionFactory(IOptions<DbOptions> options)
        => _connectionString = options.Value.ConnectionString;

    public DbConnection CreateConnection()
        => new SqliteConnection(_connectionString);
}
