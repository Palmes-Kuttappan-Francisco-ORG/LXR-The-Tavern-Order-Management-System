
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lucas_The_Tavern_Order_Management_System.Models;

public class MenuItem
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required]
    public string Category { get; set; } = "";

    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;
}

