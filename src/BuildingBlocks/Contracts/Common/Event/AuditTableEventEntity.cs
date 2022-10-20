using Contracts.Domains.Interface;

namespace Contracts.Common.Event;

public abstract class AuditTableEventEntity<T> : EventEntity<T>, IAuditable
{
    public DateTimeOffset? CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}