using Queueing.Constants;

namespace Queueing;

/// <summary>
/// Abstract class for a priority queue item. 
/// </summary>
/// <typeparam name="T"></typeparam>
public class PriorityQueueItem<T> : QueueItem<T>
{
    public int Priority { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="priority"></param>
    /// <param name="data"></param>
    /// <param name="status"></param>
    public PriorityQueueItem(T data, int priority, QueueItemStatus status = QueueItemStatus.New) 
        : base(data, status)
    {
        Priority = priority;
    }
}

