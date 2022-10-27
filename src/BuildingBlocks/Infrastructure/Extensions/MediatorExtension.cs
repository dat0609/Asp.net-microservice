using Contracts.Common.Event;
using Infrastructure.Common;
using MediatR;
using Serilog;

namespace Infrastructure.Extensions;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<BaseEvent> domainEvents, ILogger logger)
    {
        var tasks = domainEvents
            .Select(async (domainEvent) =>
            {
                await mediator.Publish(domainEvent);
                var data = new SerializerService().Serialize(domainEvent);
                logger.Information("Domain Event {@DomainEvent} published", data);
            });

        await Task.WhenAll(tasks);
    }
}