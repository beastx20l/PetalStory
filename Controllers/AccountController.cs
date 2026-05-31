using FlowerShop.Models;
using FlowerShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlowerShop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            var userIdClaim = User.FindFirstValue("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Сессия устарела. Войдите заново.";
                return RedirectToAction("Login", "Auth");
            }

            var user = _context.Users
                .Include(u => u.Orders)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден.";
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }

        // GET: /Account/Edit
        public IActionResult Edit()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email
            };

            return View(model);
        }

        // POST: /Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Профиль успешно обновлён!";
            return RedirectToAction("Profile");
        }
    }
}