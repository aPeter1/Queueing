using Queueing.Constants;
using Queueing.Sql.Sql;

namespace Queueing.Sql.TwoTableImplementation;

public class TwoTableQueue<T> : SqlPriorityQueue<T>
{
    public TwoTableQueue(string name, PriorityOrder order, SqlHelper sqlHelper, int defaultPriority = 0) 
        : base(name, order, sqlHelper, defaultPriority)
    {
    }
    
    public override Task<T?> Dequeue()
    {
        // We will want to pass the transaction to the dequeue command so it can close it.
        throw new NotImplementedException();
    }

    public override Task<bool> Enqueue(PriorityQueueItem<T> item)
    {
        throw new NotImplementedException();
    }
}