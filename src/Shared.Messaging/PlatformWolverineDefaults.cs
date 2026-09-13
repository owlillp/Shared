using RabbitMQ.Client.Exceptions;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ.Internal;

namespace Shared.Messaging;

public static class PlatformWolverineDefaults
{
    public static readonly TimeSpan DEFAULT_OUTBOX_STALE_TIME = TimeSpan.FromSeconds(5);

    public static readonly TimeSpan DEFAULT_INBOX_STALE_TIME = TimeSpan.FromMinutes(10);

    public static void UsePlatformDurabilityDefaults(this WolverineOptions opts)
    {
        opts.Durability.Mode = DurabilityMode.Solo;

        opts.Durability.OutboxStaleTime = DEFAULT_OUTBOX_STALE_TIME;
        opts.Durability.InboxStaleTime = DEFAULT_INBOX_STALE_TIME;

        opts.SendingFailure
            .OnException<BrokerUnreachableException>()
            .PauseSending(TimeSpan.FromSeconds(30));

        opts.SendingFailure
            .OnException<Exception>()
            .ScheduleRetry(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
    }

    public static RabbitMqTransportExpression UsePlatformChannelDefaults(
        this RabbitMqTransportExpression rabbit)
    {
        return rabbit.ConfigureChannelCreation(o =>
        {
            o.PublisherConfirmationsEnabled = true;
            o.PublisherConfirmationTrackingEnabled = true;
        });
    }
}