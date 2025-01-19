using System.ComponentModel.DataAnnotations;

namespace CommentApp.Models.DTOs
{
    public class CommentCreateDto
    {
        public int ItemId { get; set; }

        [Required]
        public string Text { get; set; }

        public int? ParentCommentId { get; set; }
    }
}
