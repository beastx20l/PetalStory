using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Models
{
    [Table("products")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercentage { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public int CategoryId { get; set; }
        public string? Picture { get; set; }
        public DateTime CreatedAt { get; set; }

        public Category? Category { get; set; }
    }
}