using CommentApp.Data;
using CommentApp.Models.DTOs;
using CommentApp.Models.Entities;
using CommentApp.Services.Interfaces;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CommentApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlSanitizer _sanitizer;
        private readonly IMemoryCache _cache;

        private const string CommentsVersionKey = "Comments_Version";

        public CommentService(ApplicationDbContext context, IHtmlSanitizer sanitizer, IMemoryCache cache)
        {
            _context = context;
            _sanitizer = sanitizer;
            _cache = cache;
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

            var currentVersion = _cache.Get<int?>(CommentsVersionKey) ?? 0;
            _cache.Set(CommentsVersionKey, currentVersion + 1);

            return comment;
        }

        public async Task<List<CommentDisplayDto>> GetRootCommentsAsync()
        {
            var version = _cache.Get<int?>(CommentsVersionKey) ?? 0;
            string cacheKey = $"RootComments_V{version}";

            if (!_cache.TryGetValue(cacheKey, out List<CommentDisplayDto>? cachedData) || cachedData == null)
            {
                var data = await _context.Comments
                    .AsNoTracking()
                    .Where(c => c.ParentId == null)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDisplayDto
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Text = c.Text,
                        CreatedAt = c.CreatedAt,
                        Replies = new List<CommentDisplayDto>()
                    })
                    .ToListAsync();

                _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));
                return data;
            }

            return cachedData;
        }

        public async Task<(List<CommentDisplayDto> Items, int TotalCount)> GetCommentsPagedAsync(
        int page, int pageSize, string sortBy = "date", string sortOrder = "desc")
        {
            var version = _cache.Get<int?>(CommentsVersionKey) ?? 0;

            string cacheKey = $"Comments_V{version}_P{page}_S{pageSize}_{sortBy}_{sortOrder}";

            if (_cache.TryGetValue(cacheKey, out (List<CommentDisplayDto> Items, int TotalCount) cachedData))
            {
                return cachedData;
            }

            var query = _context.Comments.AsNoTracking().Where(c => c.ParentId == null);

            query = (sortBy?.ToLower()) switch
            {
                "username" => sortOrder == "asc" ? query.OrderBy(c => c.UserName) : query.OrderByDescending(c => c.UserName),
                "email" => sortOrder == "asc" ? query.OrderBy(c => c.Email) : query.OrderByDescending(c => c.Email),
                _ => sortOrder == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var rootIds = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => c.Id)
                .ToListAsync();

            var allComments = await _context.Comments
                .AsNoTracking()
                .Select(c => new CommentDisplayDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Email = c.Email,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageUrl = c.ImagePath,
                    TextFileUrl = c.TextFilePath,
                    ParentId = c.ParentId,
                    Replies = new List<CommentDisplayDto>()
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
            }

            foreach (var id in rootIds)
            {
                if (lookup.TryGetValue(id, out var root))
                {
                    root.Replies = root.Replies.OrderBy(r => r.CreatedAt).ToList();
                    rootNodes.Add(root);
                }
            }

            var result = (rootNodes, totalCount);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}