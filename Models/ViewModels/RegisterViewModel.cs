using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class RegisterViewModel
    {
        // =========================
        // FIRST NAME
        // =========================

        [Required(ErrorMessage = "Введите имя")]

        [MinLength(2,
            ErrorMessage = "Введите корректное имя")]

        [RegularExpression(
            @"^[А-Яа-яA-Za-z]+$",
            ErrorMessage =
            "Имя должно содержать только буквы")]

        public string FirstName { get; set; }
            = string.Empty;

        // =========================
        // LAST NAME
        // =========================

        [Required(ErrorMessage = "Введите фамилию")]

        [MinLength(2,
            ErrorMessage = "Введите корректную фамилию")]

        [RegularExpression(
            @"^[А-Яа-яA-Za-z]+$",
            ErrorMessage =
            "Фамилия должна содержать только буквы")]

        public string LastName { get; set; }
            = string.Empty;

        // =========================
        // PHONE
        // =========================

        [Required(ErrorMessage = "Введите телефон")]

        [RegularExpression(
            @"^\+7\d{10}$",
            ErrorMessage =
            "Указан неверный номер")]

        public string Phone { get; set; }
            = "+7";

        // =========================
        // EMAIL
        // =========================

        [Required(ErrorMessage = "Введите email")]

        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",

            ErrorMessage =
            "Введен неверный формат")]

        public string Email { get; set; }
            = string.Empty;

        // =========================
        // PASSWORD
        // =========================

        [Required(ErrorMessage = "Введите пароль")]

        [MinLength(6,
            ErrorMessage =
            "Пароль должен содержать минимум 6 символов")]

        [RegularExpression(@"^[A-Za-z0-9]{6,}$",
        ErrorMessage = "Пароль может содержать только латинские буквы и цифры (минимум 6 символов)")]

        public string Password { get; set; }
            = string.Empty;

        // =========================
        // CONFIRM PASSWORD
        // =========================

        [Required(ErrorMessage = "Повторите пароль")]

        [Compare(
            "Password",
            ErrorMessage = "Пароли не совпадают")]

        public string ConfirmPassword { get; set; }
            = string.Empty;
    }
}
