using Queueing.Constants;

namespace Queueing;

public abstract class PriorityQueue<T> : Queue<T>
{
    protected PriorityOrder Order { get; set; }
    protected int DefaultPriority { get; set; }

    protected PriorityQueue(string name, PriorityOrder order, int defaultPriority = 0) : base(name)
    {
        Order = order;
        DefaultPriority = defaultPriority;
    }
    
    public override async Task<bool> Enqueue(T item)
    {
        return await Enqueue(new PriorityQueueItem<T>(item, DefaultPriority)).ConfigureAwait(false);
    }

    public override async Task<bool> Enqueue(QueueItem<T> item)
    {
        return await Enqueue(new PriorityQueueItem<T>(item.Data, DefaultPriority, item.Status)).ConfigureAwait(false);
    }

    public async Task<bool> Enqueue(T item, int priority)
    {
        return await Enqueue(new PriorityQueueItem<T>(item, priority)).ConfigureAwait(false);
    }

    public abstract Task<bool> Enqueue(PriorityQueueItem<T> item);
    
    public abstract override Task<T?> Dequeue();
}