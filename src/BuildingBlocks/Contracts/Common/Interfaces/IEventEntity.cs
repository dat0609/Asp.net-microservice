using Contracts.Common.Event;
using Contracts.Domains.Interface;

namespace Contracts.Common.Interfaces;

public interface IEventEntity
{
    void AddDomainEvent(BaseEvent domainEvent);
    void RemoveDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
    IReadOnlyCollection<BaseEvent> DomainEvents();
}

public interface IEventEntity<T> : IEntityBase<T>, IEventEntity
{
}