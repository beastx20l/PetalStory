using FlowerShop.Models;
using System.ComponentModel.DataAnnotations;

namespace PetalStory.Models
{
    public class UserAddress
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        public string? RecipientName { get; set; }

        public string? Phone { get; set; }

        public bool IsDefault { get; set; } = false;

        // Навигационное свойство
        public User? User { get; set; }
    }
}