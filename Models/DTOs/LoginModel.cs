using System.ComponentModel.DataAnnotations;

namespace CommentApp.Models.DTOs
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
