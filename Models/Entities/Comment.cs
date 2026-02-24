namespace CommentApp.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public string? HomePage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ParentId { get; set; }
        public Comment? Parent { get; set; }
        public List<Comment> Replies { get; set; } = new();

        public string? ImageUrl { get; set; }
        public string? TextFileUrl { get; set; }
        public string? ImagePath { get; set; }
        public string? TextFilePath { get; set; }
    }
}
