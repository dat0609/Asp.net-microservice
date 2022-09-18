namespace Basket.API.Entities;

public class Cart
{
    public string UserName { get; set; }
    public List<CartItem> Items { get; set; } = new();

    public Cart()
    {
    }

    public Cart(string userName)
    {
        UserName = userName;
    }

    public decimal TotalPrice
    {
        get
        {
            return Items.Sum(item => item.ProductPrice * item.Quantity);
        }
    }
}