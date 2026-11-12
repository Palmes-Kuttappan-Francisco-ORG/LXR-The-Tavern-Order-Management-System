using System.ComponentModel.DataAnnotations;

namespace LXR_The_Tavern_Order_Management_System.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter the customer's name.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = "";

    public string? Email { get; set; }

    public string? Phone { get; set; }
}