using FlowerShop.Models;
using PetalStory.Models;
using System.Collections.Generic;

namespace FlowerShop.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<UserAddress> Addresses { get; set; } = new List<UserAddress>();
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}