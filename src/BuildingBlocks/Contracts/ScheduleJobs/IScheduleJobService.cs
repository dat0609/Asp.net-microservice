using System.Linq.Expressions;

namespace Contracts.ScheduleJobs;

public interface IScheduleJobService
{
    #region Fire and forget
    
    public string Enqueue(Expression<Action> func);
    public string Enqueue<T>(Expression<Action<T>> func);

    #endregion

    #region Delay Job
    
    public string Schedule(Expression<Action> func, TimeSpan delay);
    public string Schedule<T>(Expression<Action<T>> func, TimeSpan delay);
    public string Schedule(Expression<Action> func, DateTimeOffset at);

    #endregion

    #region Continuos Job

    public string ContinueWith(Expression<Action> func, string parentJobId);

    #endregion
    
    public bool Delete(string jobId);
    
    public bool Requeue(string jobId);
}