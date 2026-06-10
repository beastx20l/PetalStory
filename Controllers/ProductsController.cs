using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerShop.Models;        // ← Изменили на Models

namespace FlowerShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Список всех товаров
        public async Task<IActionResult> Index(
            string? search,
            int? parentCategoryId,
            int? childCategoryId,
            int parentPage = 1,
            int childPage = 1)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            if (childCategoryId.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == childCategoryId.Value);
            }
            else if (parentCategoryId.HasValue)
            {
                var childIds = await _context.Categories
                    .Where(x => x.ParentCategoryId == parentCategoryId)
                    .Select(x => x.Id)
                    .ToListAsync();

                childIds.Add(parentCategoryId.Value);

                query = query.Where(x =>
                    childIds.Contains(x.CategoryId));
            }

            var products = await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.SelectedParentCategory = parentCategoryId;
            ViewBag.SelectedChildCategory = childCategoryId;

            var categories = await _context.Categories
                .OrderBy(x => x.Id)
                .ToListAsync();

            ViewBag.Categories = categories;
            var parentCategories = categories
    .Where(x => x.ParentCategoryId == null)
    .ToList();

            const int pageSize = 7;

            ViewBag.ParentPage = parentPage;

            ViewBag.ParentPages =
                (int)Math.Ceiling(
                    (double)parentCategories.Count / pageSize);
            if (parentCategoryId.HasValue)
            {
                var childCategories = categories
                    .Where(x => x.ParentCategoryId == parentCategoryId)
                    .ToList();

                ViewBag.ChildPage = childPage;

                ViewBag.ChildPages =
                    (int)Math.Ceiling(
                        (double)childCategories.Count / pageSize);
            }

            return View(products);
        }

        // Детальная страница одного товара
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}