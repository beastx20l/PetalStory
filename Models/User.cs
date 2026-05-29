using System.ComponentModel.DataAnnotations.Schema;


namespace FlowerShop.Models
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int RoleId { get; set; } = 1;

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Role? Role { get; set; }

        // Navigation properties
        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();
    }
}