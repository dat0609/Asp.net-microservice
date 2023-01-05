namespace Contracts.Domains.Interfaces;

public abstract class EntityAuditBase<T> : EntityBase<T>, IEntityAuditBase<T>
{
    public DateTimeOffset CreatedDate { get; set; }
    
    public DateTimeOffset? LastModifiedDate { get; set; }
}