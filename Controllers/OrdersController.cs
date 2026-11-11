
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LXR_The_Tavern_Order_Management_System.Data;
using LXR_The_Tavern_Order_Management_System.Models;

namespace LXR_The_Tavern_Order_Management_System.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // DISPLAY ALL ORDERS
        public IActionResult Index()
        {
            var orders = _db.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // DISPLAY ORDER CONFIRMATION
        public IActionResult Confirmation(int id)
        {
            var order = _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
