using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Введите телефон")]
        [RegularExpression(
            @"^\+7\d{10}$",
            ErrorMessage = "Указан неверный формат номера"
        )]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress(ErrorMessage = "Некорректный Email")]
        public string Email { get; set; } = string.Empty;
    }
}