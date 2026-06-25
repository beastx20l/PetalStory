using FlowerShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(users);
        }
        public IActionResult Details(int id)
        {
            var user = _context.Users
                .Include(x => x.Addresses)
                .Include(x => x.Orders)
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleBan(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();
            var currentUserId =
    int.Parse(User.FindFirst("UserId")!.Value);

            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "Нельзя заблокировать самого себя";

                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}