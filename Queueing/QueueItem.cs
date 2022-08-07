using Queueing.Constants;

namespace Queueing;

/// <summary>
/// Abstract class for a queue item. 
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueueItem<T>
{
    public T Data { get; set; }
    public QueueItemStatus Status { get; set; }
    
    public QueueItem(T data, QueueItemStatus status = QueueItemStatus.New)
    {
        Data = data;
        Status = status;
    }
}