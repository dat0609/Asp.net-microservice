namespace EventBus.Messages;

public interface IIntegrationEvent
{
    public DateTime CreationDate { get; init; }
    public Guid Id { get; set; }
}