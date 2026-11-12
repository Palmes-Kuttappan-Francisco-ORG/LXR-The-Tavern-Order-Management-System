
using System.ComponentModel.DataAnnotations;

namespace LXR_The_Tavern_Order_Management_System.Models;

public class CheckoutViewModel
{
    [Display(Name = "Customer Name")]
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = "";

    [Display(Name = "Payment Method")]
    [Required(ErrorMessage = "Please select a payment method.")]
    [RegularExpression("^(Cash|QRPH)$",
        ErrorMessage = "Please select Cash or QRPH.")]
    public string PaymentMethod { get; set; } = "";
}
