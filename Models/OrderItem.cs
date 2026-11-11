
namespace LXR_The_Tavern_Order_Management_System.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int MenuItemID { get; set; }

    public string MenuItemName { get; set; } = "";

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public Order Order { get; set; } = null!;
}
