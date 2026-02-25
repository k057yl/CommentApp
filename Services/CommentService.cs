using CommentApp.Data;
using CommentApp.Models.DTOs;
using CommentApp.Models.Entities;
using CommentApp.Services.Interfaces;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;

namespace CommentApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlSanitizer _sanitizer;

        public CommentService(ApplicationDbContext context, IHtmlSanitizer sanitizer)
        {
            _context = context;
            _sanitizer = sanitizer;
        }

        public async Task<Comment> CreateCommentAsync(CreateCommentRequest request, string? imagePath, string? textFilePath)
        {
            var cleanText = _sanitizer.Sanitize(request.Text);

            var comment = new Comment
            {
                UserName = request.UserName,
                Email = request.Email,
                Text = cleanText,
                ParentId = request.ParentId,
                CreatedAt = DateTime.UtcNow,
                ImagePath = imagePath,
                TextFilePath = textFilePath
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<List<Comment>> GetRootCommentsAsync()
        {
            return await _context.Comments
                .Where(c => c.ParentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.Replies)
                .ToListAsync();
        }

        public async Task<(List<CommentDisplayDto> Items, int TotalCount)> GetCommentsPagedAsync(int page, int pageSize)
        {
            var query = _context.Comments.Where(c => c.ParentId == null);
            var totalCount = await query.CountAsync();

            var rootIds = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => c.Id)
                .ToListAsync();

            var allComments = await _context.Comments
                .Select(c => new CommentDisplayDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageUrl = c.ImagePath,
                    TextFileUrl = c.TextFilePath,
                    ParentId = c.ParentId
                })
                .ToListAsync();

            var lookup = allComments.ToDictionary(c => c.Id);
            var rootNodes = new List<CommentDisplayDto>();

            foreach (var comment in allComments)
            {
                if (comment.ParentId.HasValue && lookup.TryGetValue(comment.ParentId.Value, out var parent))
                {
                    parent.Replies.Add(comment);
                }
                else if (rootIds.Contains(comment.Id))
                {
                    rootNodes.Add(comment);
                }
            }

            return (rootNodes, totalCount);
        }
    }
}