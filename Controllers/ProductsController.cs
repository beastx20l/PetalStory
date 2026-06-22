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
            int? minPrice,
            int? maxPrice,
            string? sort,
            int page = 1,
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
            if (minPrice.HasValue)
            {
                query = query.Where(x =>
                    ((x.Price ?? 0) *
                    (100 - (x.DiscountPercentage ?? 0)) / 100m)
                    >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value < 10000)
            {
                query = query.Where(x =>
                    ((x.Price ?? 0) *
                    (100 - (x.DiscountPercentage ?? 0)) / 100m)
                    <= maxPrice.Value);
            }

            switch (sort)
            {
                case "new":
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;

                case "cheap":
                    query = query.OrderBy(x =>
                        (x.Price ?? 0) *
                        (100 - (x.DiscountPercentage ?? 0)) / 100m);
                    break;

                case "expensive":
                    query = query.OrderByDescending(x =>
                        (x.Price ?? 0) *
                        (100 - (x.DiscountPercentage ?? 0)) / 100m);
                    break;

                case "discount":
                    query = query.OrderByDescending(x => x.DiscountPercentage);
                    break;
            }
            const int productsPageSize = 15;

            var totalProducts = await query.CountAsync();
            ViewBag.TotalProducts = totalProducts;
            var products = await query
                .Skip((page - 1) * productsPageSize)
                .Take(productsPageSize)
                .ToListAsync();
            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    (double)totalProducts / productsPageSize);

            ViewBag.SelectedParentCategory = parentCategoryId;
            ViewBag.SelectedChildCategory = childCategoryId;

            var categories = await _context.Categories
                .OrderBy(x => x.Id)
                .ToListAsync();

            ViewBag.Categories = categories;
            var parentCategories = categories
    .Where(x => x.ParentCategoryId == null)
    .ToList();

const int categoriesPageSize = 7;

            ViewBag.ParentPage = parentPage;

            ViewBag.ParentPages =
                (int)Math.Ceiling(
(double)parentCategories.Count / categoriesPageSize);
            if (parentCategoryId.HasValue)
            {
                var childCategories = categories
                    .Where(x => x.ParentCategoryId == parentCategoryId)
                    .ToList();

                ViewBag.ChildPage = childPage;

                ViewBag.ChildPages =
                    (int)Math.Ceiling(
(double)childCategories.Count / categoriesPageSize  );
            }
            ViewBag.MinPrice = minPrice ?? 0;
            ViewBag.MaxPrice = maxPrice ?? 10000;
            ViewBag.Sort = sort;
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