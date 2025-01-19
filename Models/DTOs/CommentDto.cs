namespace CommentApp.Models.DTOs
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
        public int? ParentCommentId { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
        public int ItemId { get; set; }
    }
}
