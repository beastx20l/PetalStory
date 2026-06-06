using FlowerShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // СПИСОК ТОВАРОВ
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }

        // GET: CREATE
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name");

            return View();
        }

        // GET: EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    _context.Categories,
                    "Id",
                    "Name");

                return View(product);
            }

            product.DiscountPercentage ??= 0;

            if (product.ImageFile != null &&
                product.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "products");

                Directory.CreateDirectory(uploadsFolder);

                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(product.ImageFile.FileName);

                string filePath =
                    Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }

                product.Picture =
                    "/images/products/" + fileName;
            }

            product.CreatedAt = DateTime.UtcNow;

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                product.DiscountPercentage ??= 0;
                ViewBag.Categories = new SelectList(
                    _context.Categories,
                    "Id",
                    "Name",
                    product.CategoryId);

                return View(product);
            }

            var dbProduct = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (dbProduct == null)
                return NotFound();

            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;
            dbProduct.Price = product.Price;
            dbProduct.StockQuantity = product.StockQuantity;
            dbProduct.DiscountPercentage =
    product.DiscountPercentage ?? 0;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.IsActive = product.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // POST: DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
