namespace EventBus.Messages;

public record IntegrationBaseEvent : IIntegrationEvent
{
    public DateTime CreationDate { get; } = DateTime.UtcNow;

    public Guid Id { get; } = Guid.NewGuid();
}