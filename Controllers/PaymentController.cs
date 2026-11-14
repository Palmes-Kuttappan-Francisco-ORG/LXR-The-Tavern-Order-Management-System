
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using LXR_The_Tavern_Order_Management_System.Data;
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
        public async Task<IActionResult> Checkout()
        {
            try
            {
                // Get cart items directly from the database
                var cartItems = await _db.CartItems.ToListAsync();

                if (!cartItems.Any())
                {
                    ViewBag.Error = "Your cart is empty.";
                    return View("Index");
                }

                // Calculate the actual cart total
                decimal amount = cartItems.Sum(item =>
                    item.Price * item.Quantity);

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

                var credentials = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(publicKey + ":"));

                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", credentials);

                var request = new MayaCheckout
                {
                    TotalAmount = new TotalAmount
                    {
                        Value = amount,
                        Currency = "PHP"
                    },

                    RequestReferenceNumber =
                        "ORD-" + Guid.NewGuid().ToString("N"),

                    RedirectUrl = new RedirectUrl
                    {
                        Success = Url.Action(
                            "Success", "Payment", null, Request.Scheme)!,

                        Failure = Url.Action(
                            "Failed", "Payment", null, Request.Scheme)!,

                        Cancel = Url.Action(
                            "Cancel", "Payment", null, Request.Scheme)!
                    }
                };

                var url =
                    "https://pg-sandbox.paymaya.com/checkout/v1/checkouts";

                var response = await _http.PostAsJsonAsync(url, request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    ViewBag.Error = "Maya API Error: " + error;
                    return View("Index");
                }

                var result = await response.Content
                    .ReadFromJsonAsync<MayaResponse>();

                if (result == null ||
                    string.IsNullOrWhiteSpace(result.RedirectUrl))
                {
                    ViewBag.Error = "Maya did not return a checkout URL.";
                    return View("Index");
                }

                return Redirect(result.RedirectUrl);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Payment error: " + ex.Message;
                return View("Index");
            }
        }

        public IActionResult Success() => View();

        public IActionResult Failed() => View();

        public IActionResult Cancel() => View();
    }
}
