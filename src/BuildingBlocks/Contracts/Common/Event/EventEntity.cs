using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace Contracts.Common.Event;

public class EventEntity<T> : EntityBase<T>, IEventEntity<T>
{
    private readonly List<BaseEvent> _events = new();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _events.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _events.Clear();
    }

    public IReadOnlyCollection<BaseEvent> DomainEvents()
    {
        return _events.AsReadOnly();
    }
}