using System.Data.SqlClient;
using System.Text.Json;
using Queueing.Constants;
using Queueing.Sql.Sql;

namespace Queueing.Sql.TwoTableImplementation;

public class TwoTableQueue<T> : SqlPriorityQueue<T>
{
    public TwoTableQueue(string name, PriorityOrder order, SqlHelper sqlHelper, int defaultPriority = 0) 
        : base(name, order, sqlHelper, defaultPriority)
    {
    }
    
    public override async Task<T?> Dequeue()
    {
        await using var connection = SqlHelper.GetSqlConnection();
        await connection.OpenAsync().ConfigureAwait(false);
        var transaction = connection.BeginTransaction();

        try
        {
            await using var command = new SqlCommand($@"
                ;with top_row as (
	                select top 1 id, status from {"meta_" + Name} with (rowlock, updlock, readpast)
	                where status = '{QueueItemStatus.New}'
	                order by priority {Order}
                )
                update top_row
                set status = '{QueueItemStatus.Pending}'
                output dq.message
                from top_row tr
                    inner join {"data_" + Name} dq on dq.id = tr.id", connection, transaction);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (reader.Read())
            {
                var item = JsonSerializer.Deserialize<T>(reader.GetString(0));
                if (item == null)
                    throw new Exception($"Error occurred deserializing a message to the specified type '{typeof(T)}'");
                await reader.CloseAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
                return item;
            }
        }
        catch
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            await connection.CloseAsync().ConfigureAwait(false);
            throw;
        }

        return default;
    }

    public override async Task<bool> Enqueue(PriorityQueueItem<T> item)
    {
        await using var connection = SqlHelper.GetSqlConnection();

        try
        {
            await connection.OpenAsync().ConfigureAwait(false);

            var message = JsonSerializer.Serialize(item.Data);
            await using var command = new SqlCommand(
                $@"insert into {"meta_" + Name}
                    (priority, status) values
                    ({item.Priority},'{item.Status}')
                    
                    declare @id int = scope_identity();

                    insert into {"data_" + Name}
                    (id, message) values 
                    (@id, '{message}');", connection);

            var rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return rowsAffected == 1;
        }
        finally
        {
            await connection.CloseAsync().ConfigureAwait(false);
        }
    }
}