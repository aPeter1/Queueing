using Queueing.Constants;
using Queueing.Sql.Sql;

namespace Queueing.Sql;

public abstract class SqlPriorityQueue<T> : PriorityQueue<T>
{
    protected readonly SqlHelper SqlHelper;
    protected SqlPriorityQueue(string name, PriorityOrder order, SqlHelper sqlHelper, int defaultPriority = 0) 
        : base(name, order, defaultPriority)
    {
        SqlHelper = sqlHelper;
    }
}