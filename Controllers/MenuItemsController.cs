using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LXR_The_Tavern_Order_Management_System.Data;
using LXR_The_Tavern_Order_Management_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace LXR_The_Tavern_Order_Management_System.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
private readonly IWebHostEnvironment _environment; 

       public MenuItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
{
    _context = context;
    _environment = environment;
}

        // GET: MenuItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.MenuItems.ToListAsync());
        }

        // GET: MenuItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // GET: MenuItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MenuItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      // POST: MenuItems/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(
    [Bind("Id,Name,Description,Price,Category,IsAvailable")] MenuItem menuItem,
    IFormFile? imageFile)
{
    ModelState.Remove(nameof(menuItem.ImageUrl));

    if (!ModelState.IsValid)
    {
        return View(menuItem);
    }

    if (imageFile != null && imageFile.Length > 0)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError("ImageUrl", "Only JPG, PNG, and WEBP images are allowed.");
            return View(menuItem);
        }

        var folderPath = Path.Combine(_environment.WebRootPath, "images", "menu");

        Directory.CreateDirectory(folderPath);

        var fileName = Guid.NewGuid().ToString() + extension;
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        menuItem.ImageUrl = "/images/menu/" + fileName;
    }

    _context.Add(menuItem);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}

        // GET: MenuItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }

        // POST: MenuItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,Name,Description,Price,Category,IsAvailable")] MenuItem menuItem,
    IFormFile? imageFile)
{
    if (id != menuItem.Id)
    {
        return NotFound();
    }

    ModelState.Remove(nameof(menuItem.ImageUrl));

    if (!ModelState.IsValid)
    {
        return View(menuItem);
    }

    var existingItem = await _context.MenuItems.FindAsync(id);

    if (existingItem == null)
    {
        return NotFound();
    }

    existingItem.Name = menuItem.Name;
    existingItem.Description = menuItem.Description;
    existingItem.Price = menuItem.Price;
    existingItem.Category = menuItem.Category;
    existingItem.IsAvailable = menuItem.IsAvailable;

    if (imageFile != null && imageFile.Length > 0)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError("ImageUrl", "Only JPG, PNG, and WEBP images are allowed.");
            return View(menuItem);
        }

        var folderPath = Path.Combine(_environment.WebRootPath, "images", "menu");

        Directory.CreateDirectory(folderPath);

        var fileName = Guid.NewGuid().ToString() + extension;
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        existingItem.ImageUrl = "/images/menu/" + fileName;
    }

    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}

        // GET: MenuItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.Id == id);
        }
    }
}
