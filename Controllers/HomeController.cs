using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LXR_The_Tavern_Order_Management_System.Models;

namespace LXR_The_Tavern_Order_Management_System.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lucas_The_Tavern_Order_Management_System.Models;

namespace Lucas_The_Tavern_Order_Management_System.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
