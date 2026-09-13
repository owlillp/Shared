using Npgsql;
using Shared.SharedKernel.Exceptions;
using Wolverine;
using Wolverine.ErrorHandling;

namespace Shared.Messaging;

public static class WolverineErrorHandlingExtensions
{
    public static void ConfigureStandardErrorPolicies(this WolverineOptions opts)
    {
        opts.Policies.OnException<TransientException>()
            .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(3))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(10));

        opts.Policies.OnException<NpgsqlException>(ex => ex.IsTransient)
            .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(3))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(10));

        opts.Policies.OnException<TimeoutException>()
            .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(3))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(15));

        opts.Policies.OnException<IOException>()
            .ScheduleRetry(TimeSpan.FromSeconds(1))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(5));

        opts.Policies.OnException<PermanentException>().MoveToErrorQueue();

        opts.Policies.OnException<ArgumentException>().MoveToErrorQueue();
        opts.Policies.OnException<ArgumentNullException>().MoveToErrorQueue();
        opts.Policies.OnException<ArgumentOutOfRangeException>().MoveToErrorQueue();
        opts.Policies.OnException<NotImplementedException>().MoveToErrorQueue();
        opts.Policies.OnException<InvalidCastException>().MoveToErrorQueue();
        opts.Policies.OnException<InvalidOperationException>().MoveToErrorQueue();

        opts.Policies.OnException<Exception>()
            .RetryWithCooldown(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(250))
            .Then.ScheduleRetry(TimeSpan.FromSeconds(5))
            .Then.MoveToErrorQueue();
    }
}