using FlowerShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetalStory.Models;

namespace FlowerShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(
                User.FindFirst("UserId")!.Value);

            var cartItems = await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            int userId = int.Parse(User.FindFirst("UserId")!.Value);

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null || !product.IsActive || product.StockQuantity <= 0)
            {
                return Json(new { success = false, message = "Товар недоступен" });
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (cartItem != null)
            {
                if (cartItem.Quantity >= product.StockQuantity)
                {
                    return Json(new { success = false, message = "Больше нет в наличии" });
                }
                cartItem.Quantity++;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    AddedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            var count = await _context.CartItems
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Quantity);

            return Json(new
            {
                success = true,
                count
            });
        }
        [HttpPost]
        public async Task<IActionResult> Increase(int cartItemId)
        {
            var item = await _context.CartItems
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == cartItemId);

            if (item == null || item.Product == null)
                return Json(new { success = false, message = "Товар не найден" });

            // Проверка наличия на складе
            if (item.Quantity >= item.Product.StockQuantity)
            {
                return Json(new
                {
                    success = false,
                    message = "Больше нет в наличии на складе"
                });
            }

            item.Quantity++;
            await _context.SaveChangesAsync();

            int userId = item.UserId;

            var cartCount = await _context.CartItems
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Quantity);

            return Json(new
            {
                success = true,
                quantity = item.Quantity,
                cartCount = cartCount
            });
        }

        [HttpPost]
        public async Task<IActionResult> Decrease(int cartItemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == cartItemId);

            if (item == null)
                return Json(new { success = false });

            if (item.Quantity > 1)
            {
                item.Quantity--;
                await _context.SaveChangesAsync();
            }

            var cartCount = await _context.CartItems
                .Where(x => x.UserId == item.UserId)
                .SumAsync(x => x.Quantity);

            return Json(new
            {
                success = true,
                quantity = item.Quantity,
                cartCount = cartCount
            });
        }
        [HttpPost]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == cartItemId);

            if (item == null)
                return Json(new { success = false });

            int userId = item.UserId;

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            var cartCount = await _context.CartItems
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Quantity);

            return Json(new
            {
                success = true,
                cartCount = cartCount
            });
        }
        public async Task<IActionResult> Checkout()
        {
            int userId =
                int.Parse(User.FindFirst("UserId")!.Value);

            var cartItems = await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] =
                    "Корзина пуста";

                return RedirectToAction("Index");
            }

            var addresses = await _context.UserAddresses
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ToListAsync();

            ViewBag.Addresses = addresses;

            decimal total = cartItems.Sum(x =>
                (x.Product!.Price ?? 0) * x.Quantity);

            ViewBag.Total = total;

            return View();
        }
        [HttpPost]
            public async Task<IActionResult> CreateOrder(
            int? addressId,
            string? newAddress,
            string? recipientName,
            string? recipientPhone,
            bool saveAddress,
            string? comment)
            {
                int userId =
                    int.Parse(User.FindFirst("UserId")!.Value);

                var cartItems = await _context.CartItems
                    .Include(x => x.Product)
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Корзина пуста"
                    });
                }

                string deliveryAddress = "";
                string customerName = "";
                string phone = "";

                if (addressId.HasValue && addressId > 0)
                {
                    var address = await _context.UserAddresses
                        .FirstOrDefaultAsync(x =>
                            x.Id == addressId &&
                            x.UserId == userId);

                    if (address == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Адрес не найден"
                        });
                    }

                    deliveryAddress = address.Address;
                    customerName = address.RecipientName ?? "";
                    phone = address.Phone ?? "";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(newAddress))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Введите адрес доставки"
                        });
                    }

                    if (string.IsNullOrWhiteSpace(recipientName))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Введите получателя"
                        });
                    }

                    if (string.IsNullOrWhiteSpace(recipientPhone))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Введите телефон"
                        });
                    }

                    deliveryAddress = newAddress;
                    customerName = recipientName;
                    phone = recipientPhone;

                    if (saveAddress)
                    {
                        _context.UserAddresses.Add(
                            new UserAddress
                            {
                                UserId = userId,
                                Address = newAddress,
                                RecipientName = recipientName,
                                Phone = recipientPhone
                            });
                    }
                }

                decimal total = cartItems.Sum(x =>
                    (x.Product!.Price ?? 0) * x.Quantity);

                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = total,
                    Status = "Новый",
                    CustomerName = customerName,
                    Phone = phone,
                    DeliveryAddress = deliveryAddress,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                foreach (var item in cartItems)
                {
                    _context.OrderItems.Add(
                        new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            ProductName = item.Product!.Name,
                            Quantity = item.Quantity,
                            PriceAtPurchase =
                                item.Product.Price ?? 0
                        });
                }

                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true
                });
            }
        }
}