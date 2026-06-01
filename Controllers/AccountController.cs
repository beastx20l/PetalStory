using FlowerShop.Models;
using FlowerShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetalStory.Models;

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
                .Include(u => u.Addresses)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Login", "Auth");

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email,
                Phone = user.Phone ?? "",
                CreatedAt = user.CreatedAt,
                Addresses = user.Addresses.ToList(),
                Orders = user.Orders.ToList()
            };

            return View(model);
        }

        // ==================== Добавление адреса ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressViewModel model)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return BadRequest("Ошибка авторизации");

            if (string.IsNullOrWhiteSpace(model.Address))
                return BadRequest("Адрес обязателен");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return BadRequest("Пользователь не найден");

            if (model.IsDefault)
            {
                var oldDefaults = _context.UserAddresses.Where(a => a.UserId == userId && a.IsDefault);
                foreach (var addr in oldDefaults)
                {
                    addr.IsDefault = false;
                }
            }

            var address = new UserAddress
            {
                UserId = userId,
                Address = model.Address.Trim(),
                IsDefault = model.IsDefault
            };

            if (model.IsForAnotherPerson)
            {
                address.RecipientName = model.RecipientName?.Trim();
                address.Phone = model.Phone?.Trim();
            }
            else
            {
                address.RecipientName = $"{user.FirstName} {user.LastName}".Trim();
                address.Phone = user.Phone;
            }

            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // ==================== Удаление адреса ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return BadRequest();

            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
                return NotFound();

            _context.UserAddresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return BadRequest();

            // Снимаем "основной" со всех адресов пользователя
            var allAddresses = _context.UserAddresses.Where(a => a.UserId == userId);
            foreach (var addr in allAddresses)
            {
                addr.IsDefault = false;
            }

            // Ставим основной выбранному
            var address = await _context.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (address != null)
            {
                address.IsDefault = true;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        // Остальные методы (EditProfile и т.д.) оставляем без изменений
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