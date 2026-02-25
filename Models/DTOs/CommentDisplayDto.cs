namespace CommentApp.Models.DTOs
{
    public class CommentDisplayDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public string? TextFileUrl { get; set; }
        public List<CommentDisplayDto> Replies { get; set; } = new();
        public int? ParentId { get; set; }
    }
}
