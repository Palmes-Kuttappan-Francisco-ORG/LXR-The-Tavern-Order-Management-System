using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using LXR_The_Tavern_Order_Management_System.Data;
using LXR_The_Tavern_Order_Management_System.Models;
using Lucas_The_Tavern_Order_Management_System.Models;

namespace Lucas_The_Tavern_Order_Management_System.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public PaymentController(
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ApplicationDbContext db)
        {
            _http = httpFactory.CreateClient();
            _config = config;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid || model.PaymentMethod != "QRPH")
                return RedirectToAction("Checkout", "Cart");

            var cartItems = await _db.CartItems.ToListAsync();

            if (!cartItems.Any())
            {
                ViewBag.Error = "Your cart is empty.";
                return View("Index");
            }

            var amount = cartItems.Sum(item => item.Price * item.Quantity);

            if (amount <= 0)
            {
                ViewBag.Error = "Invalid payment amount.";
                return View("Index");
            }

            var publicKey = _config["Maya:PublicKey"];

            if (string.IsNullOrWhiteSpace(publicKey))
            {
                ViewBag.Error = "Maya public key is missing.";
                return View("Index");
            }

            var reference = "LXR-" + Guid.NewGuid().ToString("N");

            var order = new Order
            {
                Name = model.Name.Trim(),
                PaymentMethod = "QRPH",
                OrderNumber = reference,
                OrderDate = DateTime.Now,
                TotalAmount = amount,
                PaymentStatus = "Pending",
                MayaRequestReference = reference
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            try
            {
                var credentials = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(publicKey + ":"));

                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", credentials);
var baseUrl = _config["App:BaseUrl"];

var request = new MayaCheckout
{
    TotalAmount = new TotalAmount
    {
        Value = amount,
        Currency = "PHP"
    },
    RequestReferenceNumber = reference,
    RedirectUrl = new RedirectUrl
    {
        Success = $"{baseUrl}/Payment/Success?orderId={order.Id}",
        Failure = $"{baseUrl}/Payment/Failed",
        Cancel = $"{baseUrl}/Payment/Cancel"
    }
};

                const string mayaUrl =
                    "https://pg-sandbox.paymaya.com/checkout/v1/checkouts";

                var response = await _http.PostAsJsonAsync(mayaUrl, request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    order.PaymentStatus = "Failed";
                    await _db.SaveChangesAsync();

                    ViewBag.Error = "Maya checkout could not be created: " + error;
                    return View("Index");
                }

                var result =
                    await response.Content.ReadFromJsonAsync<MayaResponse>();

                if (result == null ||
                    string.IsNullOrWhiteSpace(result.CheckoutId) ||
                    string.IsNullOrWhiteSpace(result.RedirectUrl))
                {
                    order.PaymentStatus = "Failed";
                    await _db.SaveChangesAsync();

                    ViewBag.Error = "Maya did not return a valid checkout response.";
                    return View("Index");
                }

                order.MayaCheckoutId = result.CheckoutId;
                await _db.SaveChangesAsync();

                return Redirect(result.RedirectUrl);
            }
            catch (Exception)
            {
                order.PaymentStatus = "Failed";
                await _db.SaveChangesAsync();

                ViewBag.Error = "Unable to connect to Maya. Please try again.";
                return View("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || string.IsNullOrWhiteSpace(order.MayaCheckoutId))
            {
                ViewBag.Error = "We could not find your payment.";
                return View("Index");
            }

            var publicKey = _config["Maya:PublicKey"];

            if (string.IsNullOrWhiteSpace(publicKey))
            {
                ViewBag.Error = "Payment verification is not configured.";
                return View("Index");
            }

            try
            {
                var credentials = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(publicKey + ":"));

                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", credentials);

                var statusUrl =
                    "https://pg-sandbox.paymaya.com/payments/v1/payments/" +
                    Uri.EscapeDataString(order.MayaCheckoutId) + "/status";

                var response = await _http.GetAsync(statusUrl);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Message =
                        "Your payment is being checked. Please check again shortly.";
                    return View("Index");
                }

                using var document = JsonDocument.Parse(
                    await response.Content.ReadAsStringAsync());

                var root = document.RootElement;

                string paymentStatus = "";

if (root.TryGetProperty("paymentStatus", out var paymentStatusElement))
{
    paymentStatus = paymentStatusElement.GetString() ?? "";
}
else if (root.TryGetProperty("status", out var statusElement))
{
    paymentStatus = statusElement.GetString() ?? "";
}

                if (paymentStatus == "PAYMENT_SUCCESS")
                {
                    order.PaymentStatus = "Paid";

                    
                    var cartItems = await _db.CartItems.ToListAsync();
                    _db.CartItems.RemoveRange(cartItems);

                    await _db.SaveChangesAsync();

                 return View("ThankYou", order);
                }
                else if (paymentStatus == "PAYMENT_FAILED" ||
                         paymentStatus == "PAYMENT_EXPIRED" ||
                         paymentStatus == "PAYMENT_CANCELLED")
                {
                    order.PaymentStatus = paymentStatus;
                    await _db.SaveChangesAsync();

                    ViewBag.Error = "Payment was not completed. Your cart has been preserved.";
                }
                else
{
    ViewBag.Message =
        "Maya payment status: " + paymentStatus;
}

                return View("Index");
            }
            catch
            {
                ViewBag.Message =
                    "We could not verify the payment yet. Your cart has been preserved.";
                return View("Index");
            }
        }

        public IActionResult Failed()
        {
            ViewBag.Error = "Payment was not completed. Please try again.";
            return View("Index");
        }

        public IActionResult Cancel()
        {
            ViewBag.Error = "Payment was cancelled. Your cart is still available.";
            return View("Index");
        }
    }
}