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
            ErrorMessage = "Минимум 2 символа")]

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
            ErrorMessage = "Минимум 2 символа")]

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
            "Введите номер полностью")]

        public string Phone { get; set; }
            = "+7";

        // =========================
        // EMAIL
        // =========================

        [Required(ErrorMessage = "Введите email")]

        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",

            ErrorMessage =
            "Email должен содержать только английские символы")]

        public string Email { get; set; }
            = string.Empty;

        // =========================
        // PASSWORD
        // =========================

        [Required(ErrorMessage = "Введите пароль")]

        [MinLength(6,
            ErrorMessage =
            "Пароль должен содержать минимум 6 символов")]

        [RegularExpression(
            @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&_\-])[A-Za-z\d@$!%*#?&_\-]+$",

            ErrorMessage =
            "Пароль должен содержать английские буквы, цифры и символы")]

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
