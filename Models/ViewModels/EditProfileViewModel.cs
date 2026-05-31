using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Фамилия")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        // Email только для отображения, редактировать нельзя
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}