using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
namespace FlowerShop.Models
{
    [Table("products")]
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите название товара")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Введите цену")]
        [Range(1, 1000000,
            ErrorMessage = "Цена должна быть больше 0")]
        public decimal? Price { get; set; }

        [Range(0, 100,
            ErrorMessage = "Скидка должна быть от 0 до 100")]
        public int? DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        [Range(0, 100000,
            ErrorMessage = "Количество не может быть отрицательным")]
        public int? StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        [Range(1, int.MaxValue,
            ErrorMessage = "Выберите категорию")]
        public int CategoryId { get; set; }
        public string? Picture { get; set; }
        public DateTime CreatedAt { get; set; }

        public Category? Category { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}