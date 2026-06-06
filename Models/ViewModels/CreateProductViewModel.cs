using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Введите название товара")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Введите цену")]
        public decimal? Price { get; set; }

        public int? DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        public int? StockQuantity { get; set; }

        public int CategoryId { get; set; }

        public bool IsActive { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}