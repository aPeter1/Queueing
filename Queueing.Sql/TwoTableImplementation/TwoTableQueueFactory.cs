using Queueing.Constants;

namespace Queueing.Sql.TwoTableImplementation;

public class TwoTableQueueFactory<T> : SqlPriorityQueueFactory<T>
{
    public TwoTableQueueFactory(string connectionString, int maxDataLength) 
        : base(connectionString, maxDataLength)
    {
    }
    
    public override Task<Queue<T>> GetQueueAsync(string name, PriorityOrder order = PriorityOrder.Desc, 
        int defaultPriority = 0)
    {
        throw new NotImplementedException();
    }
}