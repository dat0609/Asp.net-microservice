using System.Reflection;
using Contracts.Common.Event;
using Contracts.Common.Interfaces;
using Contracts.Domains.Interface;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Serilog;

namespace Ordering.Infrastructure.Persistence;

public class OrderContext : DbContext
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;
    private IEnumerable<BaseEvent> _events;

    private void SetBaseEventBeforeSave()
    {
        var domainEntities = ChangeTracker
            .Entries<IEventEntity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents().Any());
        
        _events = domainEntities
            .SelectMany(x => x.DomainEvents())
            .ToList();
        
        domainEntities.ToList().ForEach(entity => entity.ClearDomainEvents());
    }
    public OrderContext(DbContextOptions<OrderContext> options, IMediator mediator, ILogger logger, IEnumerable<BaseEvent> events) : base(options)
    {
        _mediator = mediator;
        _logger = logger;
        _events = events;
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetBaseEventBeforeSave();
        var modified = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified ||
                        e.State == EntityState.Added ||
                        e.State == EntityState.Deleted);

        foreach (var item in modified)
        {
            switch (item.State)
            {
                case EntityState.Added:
                    if (item.Entity is IDateTracking addedEntity)
                    {
                        addedEntity.CreatedDate = DateTime.UtcNow;
                        item.State = EntityState.Added;
                    }
                    break;

                case EntityState.Modified:
                    Entry(item.Entity).Property("Id").IsModified = false;
                    if (item.Entity is IDateTracking modifiedEntity)
                    {
                        modifiedEntity.LastModifiedDate = DateTime.UtcNow;
                        item.State = EntityState.Modified;
                    }
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        await _mediator.DispatchDomainEventsAsync(_events, _logger);
        
        return result;
    }
}