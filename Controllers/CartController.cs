
using Microsoft.AspNetCore.Mvc;
using Lucas_The_Tavern_Order_Management_System.Data;
using Lucas_The_Tavern_Order_Management_System.Models;

namespace Lucas_The_Tavern_Order_Management_System.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var cartItems = _db.CartItems.ToList();
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int menuItemId)
        {
            var menuItem = _db.MenuItems.Find(menuItemId);

            if (menuItem == null || !menuItem.IsAvailable)
                return RedirectToAction("Index", "MenuItems");

            var item = _db.CartItems
                .FirstOrDefault(c => c.MenuItemID == menuItemId);

            if (item == null)
            {
                _db.CartItems.Add(new CartItem
                {
                    MenuItemID = menuItem.Id,
                    MenuItemName = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var item = _db.CartItems.Find(id);

            if (item != null)
            {
                item.Quantity = quantity < 1 ? 1 : quantity;
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var item = _db.CartItems.Find(id);

            if (item != null)
            {
                _db.CartItems.Remove(item);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
