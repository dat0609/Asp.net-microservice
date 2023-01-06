using System.Linq.Expressions;
using Contracts.ScheduleJobs;
using Hangfire;

namespace Infrastructure.ScheduleJob;

public class HangfireService : IScheduleJobService
{
    public string Enqueue(Expression<Action> func) => BackgroundJob.Enqueue(func);

    public string Enqueue<T>(Expression<Action<T>> func) => BackgroundJob.Enqueue<T>(func);

    public string Schedule(Expression<Action> func, TimeSpan delay) => BackgroundJob.Schedule(func, delay);

    public string Schedule<T>(Expression<Action<T>> func, TimeSpan delay) => BackgroundJob.Schedule<T>(func, delay);

    public string Schedule(Expression<Action> func, DateTimeOffset at) => BackgroundJob.Schedule(func, at);

    public string ContinueWith(Expression<Action> func, string parentJobId) =>
        BackgroundJob.ContinueJobWith(parentJobId, func);

    public bool Delete(string jobId) => BackgroundJob.Delete(jobId);

    public bool Requeue(string jobId) => BackgroundJob.Requeue(jobId);
}