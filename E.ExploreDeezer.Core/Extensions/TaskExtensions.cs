using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace E.ExploreDeezer.Core.Extensions
{
    public static class TaskExtensions
    {
        public static Task ContinueWhenNotCancelled<TResult>(this Task<TResult> task, Action<Task<TResult>> action, CancellationToken cancellationToken)
            => task.ContinueWith(action, cancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        public static Task<TNewResult> ContinueWhenNotCancelled<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, TNewResult> action, CancellationToken cancellationToken)
            => task.ContinueWith(action, cancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        public static (bool failed,
                       Exception ex) CheckIfFailed<T>(this Task<T> task)
        {
            bool failed = task.IsFaulted;
            return (failed, failed ? task.Exception.GetBaseException()
                                   : null);
        }
    }
}
