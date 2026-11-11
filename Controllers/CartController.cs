
using Microsoft.AspNetCore.Mvc;
using LXR_The_Tavern_Order_Management_System.Data;
using LXR_The_Tavern_Order_Management_System.Models;

namespace LXR_The_Tavern_Order_Management_System.Controllers
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

        // DISPLAY CHECKOUT FORM
        [HttpGet]
        public IActionResult Checkout()
        {
            if (!_db.CartItems.Any())
                return RedirectToAction("Index");

            return View(new CheckoutViewModel());
        }

        // PROCESS CHECKOUT FORM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            var cartItems = _db.CartItems.ToList();

            if (!cartItems.Any())
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            var orderNumber = "LXR-" + DateTime.Now.ToString("yyyyMMddHHmmss")
                + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            var order = new Order
            {
                Name = model.Name,
                PaymentMethod = model.PaymentMethod,
                OrderNumber = orderNumber,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(item => item.Price * item.Quantity),
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    MenuItemID = item.MenuItemID,
                    MenuItemName = item.MenuItemName,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);
            _db.SaveChanges();

return RedirectToAction("Confirmation", "Orders", new { id = order.Id });        }

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
