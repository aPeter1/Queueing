using System.Data.SqlClient;
using Queueing.Constants;

namespace Queueing.Sql.Sql;

public class SqlHelper
{
    public static readonly TimeSpan SqlTimeout = new(0, 0, 60);
    private readonly string _connectionString;

    public SqlHelper(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public SqlConnection GetSqlConnection()
    {
        return new SqlConnection(_connectionString);
    }
}