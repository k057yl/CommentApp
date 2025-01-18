using System.ComponentModel.DataAnnotations;

namespace CommentApp.Models.DTOs
{
    public class ItemDTO
    {
        public int ItemId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        [Required(ErrorMessage = "CAPTCHA обязательна.")]
        public string Captcha { get; set; }
    }
}
