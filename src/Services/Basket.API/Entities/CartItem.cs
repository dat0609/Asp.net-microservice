namespace Basket.API.Entities;

public class CartItem
{
    public int Quantity { get; set; }
    public string ItemNo { get; set; }
    public int AvailableQuantity { get; set; }
    public void SetAvailableQuantity(int quantity)
    {
        AvailableQuantity = quantity;
    }
    public decimal ProductPrice { get; set; }
    public string? ProductNo { get; set; }
    public string? ProductName { get; set; }
}