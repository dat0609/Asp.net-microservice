using Contracts.Common.Events;

namespace Ordering.Domain.OrderAggregate.Events;

public class OrderDeletedEvent : BaseEvent
{
    public long Id { get; }

    public OrderDeletedEvent(long id)
    {
        Id = id;
    }
}