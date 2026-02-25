using CommentApp.Models.DTOs;
using CommentApp.Models.Entities;

namespace CommentApp.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(CreateCommentRequest request, string? imagePath, string? textFilePath);
        Task<List<CommentDisplayDto>> GetRootCommentsAsync();
        Task<(List<CommentDisplayDto> Items, int TotalCount)> GetCommentsPagedAsync(int page, int pageSize, string sortBy = "date", string sortOrder = "desc");
    }
}
