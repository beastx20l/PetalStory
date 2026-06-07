using FlowerShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return View(categories);
        }
        // GET: CREATE
        public IActionResult Create()
        {
            ViewBag.ParentCategories =
                new SelectList(
                    _context.Categories
                        .Where(x => x.ParentCategoryId == null)
                        .OrderBy(x => x.Name),
                    "Id",
                    "Name");

            return View();
        }
        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategories =
                    new SelectList(
                        _context.Categories
                            .Where(x => x.ParentCategoryId == null)
                            .OrderBy(x => x.Name),
                        "Id",
                        "Name");

                return View(category);
            }

            bool categoryExists = await _context.Categories
                .AnyAsync(x =>
                    x.Name.ToLower() ==
                    category.Name.ToLower());

            if (categoryExists)
            {
                ModelState.AddModelError(
                    "Name",
                    "Категория с таким названием уже существует");

                ViewBag.ParentCategories =
                    new SelectList(
                        _context.Categories
                            .Where(x => x.ParentCategoryId == null)
                            .OrderBy(x => x.Name),
                        "Id",
                        "Name");

                return View(category);
            }

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            ViewBag.ParentCategories = new SelectList(
                _context.Categories
                    .Where(x =>
                        x.ParentCategoryId == null &&
                        x.Id != id),
                "Id",
                "Name",
                category.ParentCategoryId);

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategories =
                    new SelectList(
                        _context.Categories
                            .Where(x =>
                                x.ParentCategoryId == null &&
                                x.Id != category.Id),
                        "Id",
                        "Name",
                        category.ParentCategoryId);

                return View(category);
            }

            var dbCategory = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == category.Id);

            if (dbCategory == null)
                return NotFound();

            bool categoryExists = await _context.Categories
                .AnyAsync(x =>
                    x.Id != category.Id &&
                    x.Name.ToLower() ==
                    category.Name.ToLower());

            if (categoryExists)
            {
                ModelState.AddModelError(
                    "Name",
                    "Категория с таким названием уже существует");

                ViewBag.ParentCategories =
                    new SelectList(
                        _context.Categories
                            .Where(x =>
                                x.ParentCategoryId == null &&
                                x.Id != category.Id),
                        "Id",
                        "Name",
                        category.ParentCategoryId);

                return View(category);
            }

            bool hasChildren = await _context.Categories
                .AnyAsync(x => x.ParentCategoryId == category.Id);

            if (hasChildren &&
                dbCategory.ParentCategoryId != category.ParentCategoryId)
            {
                ModelState.AddModelError(
                    "ParentCategoryId",
                    "Нельзя менять родителя категории, у которой есть подкатегории");

                ViewBag.ParentCategories =
                    new SelectList(
                        _context.Categories
                            .Where(x =>
                                x.ParentCategoryId == null &&
                                x.Id != category.Id),
                        "Id",
                        "Name",
                        category.ParentCategoryId);

                return View(category);
            }

            if (category.ParentCategoryId.HasValue)
            {
                int? parentId = category.ParentCategoryId;

                while (parentId != null)
                {
                    if (parentId == category.Id)
                    {
                        ModelState.AddModelError(
                            "ParentCategoryId",
                            "Нельзя создать циклическую ссылку категорий");

                        ViewBag.ParentCategories =
                            new SelectList(
                                _context.Categories
                                    .Where(x =>
                                        x.ParentCategoryId == null &&
                                        x.Id != category.Id),
                                "Id",
                                "Name",
                                category.ParentCategoryId);

                        return View(category);
                    }

                    parentId = await _context.Categories
                        .Where(x => x.Id == parentId)
                        .Select(x => x.ParentCategoryId)
                        .FirstOrDefaultAsync();
                }
            }

            dbCategory.Name = category.Name;
            dbCategory.ParentCategoryId = category.ParentCategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}