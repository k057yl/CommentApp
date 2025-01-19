using CommentApp.Data;
using CommentApp.Models.DTOs;
using CommentApp.Models.Entities;
using CommentApp.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommentApp.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CaptchaService _captchaService;

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager, CaptchaService captchaService)
        {
            _context = context;
            _userManager = userManager;
            _captchaService = captchaService;
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var posts = await _context.Items
                .Include(i => i.User)
                .OrderByDescending(i => i.CreationDate)
                .ToListAsync();

            return View(posts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ItemDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemDTO itemDto, string Captcha)
        {
            var captchaCode = HttpContext.Session.GetString("CaptchaCode");

            // Проверяем CAPTCHA
            if (captchaCode == null || itemDto.Captcha != captchaCode)
            {
                ModelState.AddModelError("Captcha", "Неверный код CAPTCHA.");
            }

            if (string.IsNullOrEmpty(Captcha) || Captcha != captchaCode)
            {
                ModelState.AddModelError("Captcha", "Неверный код CAPTCHA1111.");
            }

            // Если ModelState не валиден
            if (!ModelState.IsValid)
            {
                return View(itemDto);
            }

            var user = await _userManager.GetUserAsync(User);
            var item = new Item
            {
                Name = itemDto.Name,
                Text = itemDto.Text,
                CreationDate = DateTime.UtcNow,
                UserId = user.Id
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Items
                .Include(i => i.User)
                .Include(i => i.Comments)
                .ThenInclude(c => c.Replies)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            var commentsDto = item.Comments
                .Where(c => c.ParentCommentId == null)
                .Select(c => MapToDto(c))
                .ToList();

            ViewBag.Comments = commentsDto;
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CommentCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { id = model.ItemId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                ItemId = model.ItemId,
                UserId = user.Id,
                Text = model.Text,
                CreationDate = DateTime.UtcNow,
                ParentCommentId = model.ParentCommentId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.ItemId });
        }

        private CommentDto MapToDto(Comment comment)
        {
            return new CommentDto
            {
                CommentId = comment.CommentId,
                Text = comment.Text,
                UserName = comment.User.UserName,
                CreationDate = comment.CreationDate,
                ParentCommentId = comment.ParentCommentId,
                Replies = comment.Replies.Select(MapToDto).ToList()
            };
        }
    }
}