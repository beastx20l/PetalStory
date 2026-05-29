using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Models
{
    [Table("categories")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
    }
}