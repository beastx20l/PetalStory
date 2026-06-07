using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Models
{
    [Table("categories")]
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название категории")]
        [StringLength(100,
            ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;

        public int? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }
        public ICollection<Product> Products { get; set; }
        = new List<Product>();
    }
}