using Queueing.Constants;

namespace Queueing;

public abstract class PriorityQueueFactory<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// /// <param name="order"></param>
    /// <param name="defaultPriority"></param>
    /// <returns></returns>
    public Task<PriorityQueue<T>> GetQueueAsync(PriorityOrder order = PriorityOrder.Desc, int defaultPriority = 0)
    {
        return GetQueueAsync(QueueNameFromType(), order, defaultPriority);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="order"></param>
    /// <param name="defaultPriority"></param>
    /// <returns></returns>
    public abstract Task<PriorityQueue<T>> GetQueueAsync(string name, PriorityOrder order = PriorityOrder.Desc,
        int defaultPriority = 0);
    
    protected static string QueueNameFromType()
    {
        return typeof(T).ToString().ToLower() + "_queue";
    }
}