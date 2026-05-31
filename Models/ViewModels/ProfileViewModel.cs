using PetalStory.Models;

namespace PetalStory.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }           // Дата регистрации

        public List<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    }
}