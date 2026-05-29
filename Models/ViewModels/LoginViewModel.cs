using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
            = string.Empty;

        [Required]
        public string Password { get; set; }
            = string.Empty;
    }
}
