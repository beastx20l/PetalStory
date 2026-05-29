using FlowerShop.Models;
using FlowerShop.Models.ViewModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace FlowerShop.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LOGIN
        // =========================

        // GET
        public IActionResult Login()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u =>
                u.Email == model.Email &&
                u.Password == model.Password);

            if (user == null)
            {
                ViewBag.Error =
                    "Неверный email или пароль";

                return View(model);
            }

            var claims =
                new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(
                    ClaimTypes.Name,
                    user.Email),

                new System.Security.Claims.Claim(
                    "UserId",
                    user.Id.ToString()),

                new System.Security.Claims.Claim(
                    ClaimTypes.Role,

                    user.RoleId switch
                    {
                        3 => "Admin",
                        2 => "Manager",
                        _ => "Customer"
                    })
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal =
                new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =========================
        // REGISTER
        // =========================

        // GET
        public IActionResult Register()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Проверка email
            var existingUser =
                _context.Users.FirstOrDefault(u =>
                    u.Email == model.Email);

            if (existingUser != null)
            {
                ViewBag.Error =
                    "Пользователь с таким email уже существует";

                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,

                Email = model.Email,
                Password = model.Password,

                RoleId = 1,

                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // =========================
        // LOGOUT
        // =========================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(
                "Index",
                "Home");
        }
    }
}
