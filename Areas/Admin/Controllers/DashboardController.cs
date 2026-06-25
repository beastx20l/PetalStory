using FlowerShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlowerShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    [Authorize]   // Убрали Roles = "Admin", чтобы менеджеры тоже заходили
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var utcNow = DateTime.UtcNow;
            var todayUtc = utcNow.Date;
            var startOfMonthUtc = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // Получаем роль текущего пользователя из Claims
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isAdmin = roleClaim == "Admin";

            var model = new
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                MonthlyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= startOfMonthUtc)
                    .SumAsync(o => o.TotalAmount),
                DailyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Date == todayUtc)
                    .SumAsync(o => o.TotalAmount),

                // Передаём в представление
                IsAdmin = isAdmin
            };

            return View(model);
        }
    }
}