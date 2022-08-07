using System.Data;
using System.Data.SqlClient;
using Queueing.Constants;

namespace Queueing.Sql.OneTableImplementation;

public class OneTableQueueFactory<T> : SqlPriorityQueueFactory<T>
{
    private readonly Dictionary<string, string> _tableSchema;
    
    public OneTableQueueFactory(string connectionString, int maxDataLength) : base(connectionString, maxDataLength)
    {
        _tableSchema = new Dictionary<string, string>
        {
            { "id", "int IDENTITY(1,1) NOT NULL" },
            { "priority", "int NOT NULL" },
            { "status", "nvarchar(10) NOT NULL" },
            { "message", $"nvarchar({MaxDataLength}) NOT NULL" }
        };
    }
    
    public override async Task<Queue<T>> GetQueueAsync(string name, PriorityOrder order = PriorityOrder.Desc,
        int defaultPriority = 0)
    {
        if (!await VerifyTableExists(name, _tableSchema).ConfigureAwait(false))
            await CreateOtqTable(name, order).ConfigureAwait(false);
        
        return new OneTableQueue<T>(name, order, SqlHelper, defaultPriority);
    }

    private async Task CreateOtqTable(string name, PriorityOrder priority)
    {
        await using var connection = SqlHelper.GetSqlConnection();

        try
        {
            await connection.OpenAsync().ConfigureAwait(false);
            await using var command = new SqlCommand(
                $@"
                CREATE TABLE {name}
                (id int IDENTITY(1,1) NOT NULL,
                    priority int,
                    status nvarchar(10),
                    message nvarchar({MaxDataLength}))
                ON [PRIMARY]"
                , connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            
            await using var constraintCommand = new SqlCommand(
                $@"
                    CREATE UNIQUE CLUSTERED INDEX [PK_{name}] ON {name}
                (
                    [id] ASC
                )
                    CREATE NONCLUSTERED INDEX [IX_{name}] ON {name}
                (
                    [priority] {priority},
                    [status] {priority}
                )"
                , connection);
            await constraintCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        finally
        {
            await connection.CloseAsync().ConfigureAwait(false);
        }
    }
}