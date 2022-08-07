using System.Data.SqlClient;
using Queueing.Constants;

namespace Queueing.Sql.TwoTableImplementation;

public class TwoTableQueueFactory<T> : SqlPriorityQueueFactory<T>
{
    private readonly Dictionary<string, string> _dataTableSchema;
    private readonly Dictionary<string, string> _metaTableSchema;
    
    public TwoTableQueueFactory(string connectionString, int maxDataLength) 
        : base(connectionString, maxDataLength)
    {
        _dataTableSchema = new Dictionary<string, string>
        {
            { "id", "int IDENTITY(1,1) NOT NULL" },
            { "message", $"nvarchar({MaxDataLength}) NOT NULL" }
        };
        
        _metaTableSchema = new Dictionary<string, string>
        {
            { "id", "int IDENTITY(1,1) NOT NULL" },
            { "priority", "int NOT NULL" },
            { "status", "nvarchar(10) NOT NULL" }
        };
    }
    
    public override async Task<PriorityQueue<T>> GetQueueAsync(string name, PriorityOrder order = PriorityOrder.Desc, 
        int defaultPriority = 0)
    {
        if (!await VerifyTableExists("data_" + name, _dataTableSchema).ConfigureAwait(false))
            await CreateDataTable("data_" + name).ConfigureAwait(false);
        
        if (!await VerifyTableExists("meta_" + name, _metaTableSchema).ConfigureAwait(false))
            await CreateMetaTable("meta_" + name, order).ConfigureAwait(false);
        
        return new TwoTableQueue<T>(name, order, SqlHelper, defaultPriority);
    }

    private async Task CreateDataTable(string name)
    {
        await using var connection = SqlHelper.GetSqlConnection();

        try
        {
            await connection.OpenAsync().ConfigureAwait(false);
            await using var command = new SqlCommand(
                $@"
                CREATE TABLE {name}
                (id int NOT NULL,
                    message nvarchar({MaxDataLength}) NOT NULL)
                ON [PRIMARY]"
                , connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            
            await using var constraintCommand = new SqlCommand(
                $@"
                    CREATE UNIQUE NONCLUSTERED INDEX [PK_{name}] ON {name}
                (
                    [id] ASC
                )"
                , connection);
            await constraintCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        finally
        {
            await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    private async Task CreateMetaTable(string name, PriorityOrder order)
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
                 CONSTRAINT [PK_{name}] PRIMARY KEY CLUSTERED
                (
                    [id] {order}
                ))"
                , connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            
            await using var constraintCommand = new SqlCommand(
                $@"CREATE NONCLUSTERED INDEX [IX_{name}] ON {name}
                (
                    [priority] {order},
                    [status] {order}
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