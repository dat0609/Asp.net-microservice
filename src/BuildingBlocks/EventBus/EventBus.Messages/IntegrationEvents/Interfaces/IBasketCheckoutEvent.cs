namespace EventBus.Messages.IntegrationEvents.Events;

public interface IBasketCheckoutEvent : IIntegrationEvent
{
    string UserName { get; set; }
    decimal TotalPrice { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string EmailAddress { get; set; }
    public string ShippingAddress { get; set; }
    public string InvoiceAddress { get; set; }
}