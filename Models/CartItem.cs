using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Models
{
    [Table("cartitems")]
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; }

        public Product? Product { get; set; }
    }
}