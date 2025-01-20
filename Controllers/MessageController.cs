using CommentApp.Data;
using CommentApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using CommentApp.Resources;

namespace CommentApp.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HtmlValidator _htmlValidator;

        public MessageController(ApplicationDbContext context, UserManager<IdentityUser> userManager, HtmlValidator htmlValidator)
        {
            _context = context;
            _userManager = userManager;
            _htmlValidator = htmlValidator;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int itemId, string text, IFormFile attachment, int? parentCommentId)
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

            if (!_htmlValidator.ValidateHtml(text))
            {
                ModelState.AddModelError("", "Сообщение содержит недопустимые теги или атрибуты.");
                return RedirectToAction("ShowAll", "Item", new { id = itemId });
            }

            var sanitizer = new HtmlSanitizer();
            var sanitizedText = sanitizer.Sanitize(text);

            string attachmentPath = null;
            if (attachment != null && attachment.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(attachment.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "Недопустимый формат файла. Допустимые форматы: JPG, PNG, GIF.");
                    return RedirectToAction("ShowAll", "Item", new { id = itemId });
                }

                using (var imageStream = attachment.OpenReadStream())
                {
                    var image = System.Drawing.Image.FromStream(imageStream);
                    int maxWidth = 320;
                    int maxHeight = 240;

                    if (image.Width > maxWidth || image.Height > maxHeight)
                    {
                        var ratioX = (double)maxWidth / image.Width;
                        var ratioY = (double)maxHeight / image.Height;
                        var ratio = Math.Min(ratioX, ratioY);

                        var newWidth = (int)(image.Width * ratio);
                        var newHeight = (int)(image.Height * ratio);

                        var resizedImage = new Bitmap(image, newWidth, newHeight);

                        var fileName = Path.GetFileName(attachment.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                        resizedImage.Save(filePath, image.RawFormat);

                        attachmentPath = $"/images/{fileName}";
                    }
                    else
                    {
                        var fileName = Path.GetFileName(attachment.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                        image.Save(filePath, image.RawFormat);

                        attachmentPath = $"/images/{fileName}";
                    }
                }
            }

            var comment = new Comment
            {
                ItemId = itemId,
                UserId = user.Id,
                Text = sanitizedText,
                CreationDate = DateTime.UtcNow,
                ParentCommentId = parentCommentId,
                AttachmentPath = attachmentPath
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