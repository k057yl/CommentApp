namespace CommentApp.Models.DTOs
{
    public class CreateCommentRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? HomePage { get; set; }
        public string Text { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string Captcha { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public IFormFile? TextFile { get; set; }
    }
}
