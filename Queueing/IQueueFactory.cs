using Queueing.Constants;

namespace Queueing;

public interface IQueueFactory<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// /// <param name="order"></param>
    /// <param name="defaultPriority"></param>
    /// <returns></returns>
    public abstract Task<Queue<T>> GetQueueAsync(PriorityOrder order = PriorityOrder.Desc, int defaultPriority = 0);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="order"></param>
    /// <param name="defaultPriority"></param>
    /// <returns></returns>
    public abstract Task<Queue<T>> GetQueueAsync(string name, PriorityOrder order = PriorityOrder.Desc, int defaultPriority = 0);
}