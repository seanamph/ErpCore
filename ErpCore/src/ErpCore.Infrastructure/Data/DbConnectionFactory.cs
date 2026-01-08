using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ErpCore.Infrastructure.Data;

/// <summary>
/// SQL Server 資料庫連線工廠實作
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}

