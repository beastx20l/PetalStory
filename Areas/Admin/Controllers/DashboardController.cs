using FlowerShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
            var todayUtc = utcNow.Date;                    // Сегодня в UTC
            var startOfMonthUtc = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var model = new
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),

                MonthlyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= startOfMonthUtc)
                    .SumAsync(o => o.TotalAmount),

                DailyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Date == todayUtc)   // или o.CreatedAt >= todayUtc && o.CreatedAt < todayUtc.AddDays(1)
                    .SumAsync(o => o.TotalAmount)
            };

            return View(model);
        }
    }
}