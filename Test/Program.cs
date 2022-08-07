// See https://aka.ms/new-console-template for more information

using System.Data.SqlClient;
using Queueing.Sql.OneTableImplementation;
using Queueing.Sql.Sql;
using Queueing.Sql.TwoTableImplementation;

var connectionString = "Data Source=alec-precision\\brevium2012;Initial Catalog=Testing;user id=sa;password=Brevium2;";

var factory = new OneTableQueueFactory<Item>(connectionString, 100);
var queue = await factory.GetQueueAsync("new_item_queue").ConfigureAwait(false);

var watch = new System.Diagnostics.Stopwatch();
var reps = 10000;
watch.Start();
for(var i = 0; i < reps; i++)
{
    if (i % (reps/10) == 0)
    {
        Console.WriteLine($"Execution Time Enqueue: {watch.ElapsedMilliseconds} ms | {i}");
        watch.Restart();
    }

    await queue.Enqueue(new Item
    {
        Other = i,
        Thing = $"This is my data! {i}"
    }, i).ConfigureAwait(false);
}
watch.Stop();
Console.WriteLine($"Execution Time Enqueue: {watch.ElapsedMilliseconds} ms");

watch.Restart();
for(var i = 0; i < reps; i++)
{
    if (i % (reps/10) == 0)
    {
        Console.WriteLine($"Execution Time Dequeue: {watch.ElapsedMilliseconds} ms| {reps - i}");
        watch.Restart();
    }
    await queue.Dequeue().ConfigureAwait(false);
}
watch.Stop();
Console.WriteLine($"Execution Time Dequeue: {watch.ElapsedMilliseconds} ms");


internal class Item
{
    public int Other { get; set; }
    public string? Thing { get; set; }
}