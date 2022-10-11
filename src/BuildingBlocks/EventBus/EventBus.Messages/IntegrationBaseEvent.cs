namespace EventBus.Messages;

public record IntegrationBaseEvent() : IIntegrationEvent
{
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
    public Guid Id { get; set; }
}