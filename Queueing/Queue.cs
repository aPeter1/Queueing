namespace Queueing;

public abstract class  Queue<T>
{
    public readonly string Name;

    protected Queue(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract Task<T?> Dequeue();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract Task<bool> Enqueue(T item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract Task<bool> Enqueue(QueueItem<T> item);
}