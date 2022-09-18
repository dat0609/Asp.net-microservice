namespace Basket.API.Entities;

public class CartItem
{
    public int Quantity { get; set; }
    public decimal ProductPrice { get; set; }
    public string? ProductNo { get; set; }
    public string? ProductName { get; set; }
}