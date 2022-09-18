namespace Basket.API.Entities;

public class BasketCheckout
{
    public string? UserName { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
}