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
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();

                    return BadRequest(new { success = false, message = "Ошибка валидации", errors });
                }

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return BadRequest(new { success = false, message = "Ошибка авторизации" });

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return BadRequest(new { success = false, message = "Пользователь не найден" });

                if (model.IsDefault)
                {
                    var oldDefaults = await _context.UserAddresses
                        .Where(a => a.UserId == userId && a.IsDefault)
                        .ToListAsync();

                    foreach (var a in oldDefaults)
                        a.IsDefault = false;
                }

                var hasAddresses = await _context.UserAddresses
                    .AnyAsync(a => a.UserId == userId);

                var address = new UserAddress
                {
                    UserId = userId,
                    Address = model.Address.Trim(),

                    // первый адрес всегда основной
                    IsDefault = !hasAddresses || model.IsDefault
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

                return Ok(new { success = true, message = "Адрес успешно добавлен!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ==================== Удаление адреса ====================
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Ошибка авторизации"
                });
            }

            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Адрес не найден"
                });
            }

            var addressCount = await _context.UserAddresses
                .CountAsync(a => a.UserId == userId);

            if (addressCount <= 1)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Нельзя удалить единственный сохранённый адрес"
                });
            }
            if (address.IsDefault)
            {
                var newDefault = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.Id != address.Id)
                    .OrderBy(a => a.Id)
                    .FirstOrDefaultAsync();

                if (newDefault != null)
                {
                    newDefault.IsDefault = true;
                }
            }
            _context.UserAddresses.Remove(address);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Адрес успешно удалён"
            });
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
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
        public IActionResult ChangePassword()
        {
            return PartialView(
                "_ChangePasswordModal",
                new ChangePasswordViewModel()
            );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
    ChangePasswordViewModel model)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim)
                || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest();
            }

            var user = _context.Users.Find(userId);

            if (user == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState
                        .Where(x => x.Value != null &&
                                    x.Value.Errors.Count > 0)
                        .ToDictionary(
                            k => k.Key,
                            v => v.Value!.Errors.First().ErrorMessage
                        )
                });
            }

            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError(
                    "CurrentPassword",
                    "Указан неверный текущий пароль"
                );
            }

            if (model.NewPassword == user.Password)
            {
                ModelState.AddModelError(
                    "NewPassword",
                    "Новый пароль должен отличаться от текущего"
                );
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState
                        .Where(x => x.Value != null &&
                                    x.Value.Errors.Count > 0)
                        .ToDictionary(
                            k => k.Key,
                            v => v.Value!.Errors.First().ErrorMessage
                        )
                });
            }

            user.Password = model.NewPassword;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Пароль успешно изменён"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return BadRequest();

            var user = _context.Users.Find(userId);
            if (user == null) return BadRequest();
            // ===== Имя =====
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                ModelState.AddModelError(
                    "FirstName",
                    "Введите имя"
                );
            }
            else
            {
                if (model.FirstName.Trim().Length < 2)
                {
                    ModelState.AddModelError(
                        "FirstName",
                        "Имя должно содержать минимум 2 буквы"
                    );
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    model.FirstName,
                    @"^[А-Яа-яЁёA-Za-z-]+$"))
                {
                    ModelState.AddModelError(
                        "FirstName",
                        "Имя может содержать только буквы"
                    );
                }
            }

            // ===== Фамилия =====
            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                ModelState.AddModelError(
                    "LastName",
                    "Введите фамилию"
                );
            }
            else
            {
                if (model.LastName.Trim().Length < 2)
                {
                    ModelState.AddModelError(
                        "LastName",
                        "Фамилия должна содержать минимум 2 буквы"
                    );
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    model.LastName,
                    @"^[А-Яа-яЁёA-Za-z-]+$"))
                {
                    ModelState.AddModelError(
                        "LastName",
                        "Фамилия может содержать только буквы"
                    );
                }
            }
            // Проверка Email
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Введите Email"
                );
            }
            else
            {
                model.Email = model.Email.Trim();

                // Проверка на русские буквы
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    model.Email,
                    @"[А-Яа-яЁё]"))
                {
                    ModelState.AddModelError(
                        "Email",
                        "Email должен содержать только латинские буквы"
                    );
                }
                // Проверка формата Email
                else if (!System.Text.RegularExpressions.Regex.IsMatch(
                    model.Email,
                    @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
                {
                    ModelState.AddModelError(
                        "Email",
                        "Неверный формат Email"
                    );
                }
            }

            // Проверка Email на дубликат
            if (_context.Users.Any(x =>
                x.Email == model.Email &&
                x.Id != userId))
            {
                ModelState.AddModelError(
                    "Email",
                    "Пользователь с таким Email уже существует"
                );
            }

            // Проверка телефона
            if (_context.Users.Any(x =>
                x.Phone == model.Phone &&
                x.Id != userId))
            {
                ModelState.AddModelError(
                    "Phone",
                    "Пользователь с таким номером уже существует"
                );
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState
                        .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                        .ToDictionary(
                            k => k.Key,
                            v => v.Value!.Errors.First().ErrorMessage
                        )
                });
            }

            user.FirstName = model.FirstName?.Trim();
            user.LastName = model.LastName?.Trim();
            user.Phone = model.Phone?.Trim();
            user.Email = model.Email?.Trim() ?? "";

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Профиль успешно обновлён!" });
        }
    }
}