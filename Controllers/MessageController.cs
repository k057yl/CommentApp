using CommentApp.Data;
using CommentApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using CommentApp.Models.DTOs;

namespace CommentApp.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MessageController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int itemId, string text, int? parentCommentId)
        {
            if (string.IsNullOrEmpty(text))
            {
                ModelState.AddModelError("", "Комментарий не может быть пустым.");
                return RedirectToAction("ShowAll", "Item", new { id = itemId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
            {
                return NotFound();
            }

            var sanitizer = new HtmlSanitizer();
            string sanitizedText = sanitizer.Sanitize(text);

            var comment = new Comment
            {
                ItemId = itemId,
                UserId = user.Id,
                Text = sanitizedText,
                CreationDate = DateTime.UtcNow,
                ParentCommentId = parentCommentId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("ShowAll", "Item", new { id = itemId });
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll(string sortOrder)
        {
            ViewData["UserNameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "user_name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var items = await _context.Items
                .Include(i => i.Comments)
                .ThenInclude(c => c.User)
                .ToListAsync();

            foreach (var item in items)
            {
                var commentsQuery = item.Comments.AsQueryable();

                switch (sortOrder)
                {
                    case "user_name_desc":
                        commentsQuery = commentsQuery.OrderByDescending(c => c.User.UserName);
                        break;
                    case "Date":
                        commentsQuery = commentsQuery.OrderBy(c => c.CreationDate);
                        break;
                    case "date_desc":
                        commentsQuery = commentsQuery.OrderByDescending(c => c.CreationDate);
                        break;
                    default:
                        commentsQuery = commentsQuery.OrderBy(c => c.User.UserName);
                        break;
                }

                item.Comments = commentsQuery.ToList();
            }

            return View(items);
        }
    }
}