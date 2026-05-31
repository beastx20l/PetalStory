using FlowerShop.Models;
using FlowerShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetalStory.Models.ViewModels;

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

        // GET: Личный кабинет
        public IActionResult Profile()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = _context.Users
                .Include(u => u.Orders)
                .Include(u => u.Addresses)        // ← Добавили подгрузку адресов
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login", "Auth");

            // Маппим в ViewModel
            var model = new ProfileViewModel
            {
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email,
                Phone = user.Phone ?? "",
                CreatedAt = user.CreatedAt,
                Addresses = user.Addresses.ToList()
            };

            return View(model);
        }

        // GET: Редактирование профиля (для модального окна)
        public IActionResult EditProfile()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return BadRequest();

            var user = _context.Users.Find(userId);
            if (user == null) return BadRequest();

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email
            };

            return PartialView("_EditProfileModal", model);
        }

        // POST: Сохранение профиля
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditProfileModal", model);

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return BadRequest();

            var user = _context.Users.Find(userId);
            if (user == null) return BadRequest();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Профиль успешно обновлён!" });
        }
    }
}