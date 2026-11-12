
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LXR_The_Tavern_Order_Management_System.Models;

[Index(nameof(OrderNumber), IsUnique = true)]
public class Order
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please select a payment method.")]
    public string PaymentMethod { get; set; } = "";

    [Required]
    public string OrderNumber { get; set; } = "";

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
