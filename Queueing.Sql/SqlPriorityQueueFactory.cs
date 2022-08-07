using System.Data.SqlClient;
using Queueing.Constants;
using Queueing.Sql.Sql;

namespace Queueing.Sql;

public abstract class SqlPriorityQueueFactory<T> : IQueueFactory<T>
{
    protected readonly SqlHelper SqlHelper;
    protected readonly int MaxDataLength;

    protected SqlPriorityQueueFactory(string connectionString, int maxDataLength)
    {
        SqlHelper = new SqlHelper(connectionString);
        MaxDataLength = maxDataLength;
    }
    
    public Task<Queue<T>> GetQueueAsync(PriorityOrder order = PriorityOrder.Desc, int defaultPriority = 0)
    {
        return GetQueueAsync(QueueNameFromType(), order, defaultPriority);
    }

    public abstract Task<Queue<T>> GetQueueAsync(string name, PriorityOrder order = PriorityOrder.Desc,
        int defaultPriority = 0);
    
    private static string QueueNameFromType()
    {
        return typeof(T).ToString().ToLower() + "_queue";
    }

    private async Task<bool> CheckIfTableExists(string name)
    {
        await using var connection = SqlHelper.GetSqlConnection();
        
        try
        {
            await connection.OpenAsync().ConfigureAwait(false);

            await using var command = new SqlCommand(
                $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{name}'", connection);
            await using var reader = command.ExecuteReaderAsync().WaitAsync(SqlHelper.SqlTimeout).Result;

            return reader.Read();
        }
        finally
        {
            await connection.CloseAsync().ConfigureAwait(false);
        }
    }
    
    private async Task<bool> TableSchemaMatchesType(string name, Dictionary<string, string> schema)
    {
        await using var connection = SqlHelper.GetSqlConnection();
        
        try
        {
            await connection.OpenAsync().ConfigureAwait(false);

            await using var command = new SqlCommand(
                $"SELECT COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA. COLUMNS where table_schema = 'dbo' " +
                $"and table_name = '{name}'", connection);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            var numColumns = 0;
            while (reader.Read())
            {
                var columnName = reader.GetString(0);
                var columnType = reader.GetString(1);

                if (!schema.TryGetValue(columnName, out var propertyType) || !propertyType.Contains(columnType))
                    return false;

                numColumns++;
            }
            
            return numColumns == schema.Count;
        }
        finally
        {
            await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    protected async Task<bool> VerifyTableExists(string name, Dictionary<string, string> schema)
    {
        var tableExists = await CheckIfTableExists(name).ConfigureAwait(false);

        if (tableExists)
            if (!await TableSchemaMatchesType(name, schema).ConfigureAwait(false))
                throw new Exception($"Queue table with name '{name}' exists but does not match schema of" +
                                    $" the item type provided ('{typeof(T)}')");

        return tableExists;
    }
}