using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введите текущий пароль")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите новый пароль")]
        [MinLength(6,
            ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [RegularExpression(
        @"^(?=.*[A-Za-z])(?=.*\d).{6,}$",
        ErrorMessage = "Пароль должен содержать латинские буквы и цифры")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Повторите новый пароль")]
        [Compare(
            "NewPassword",
            ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}