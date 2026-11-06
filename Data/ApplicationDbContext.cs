
using Lucas_The_Tavern_Order_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Lucas_The_Tavern_Order_Management_System.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; }

public DbSet<Customer> Customer { get; set; } = default!;
}
