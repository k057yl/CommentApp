using CommentApp.Data;
using CommentApp.Models.DTOs;
using CommentApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ganss.Xss;

namespace CommentApp.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> ShowAll()
        {
            var posts = await _context.Items
                .Include(i => i.User)
                .Include(i => i.Comments)
                    .ThenInclude(c => c.User)
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
            var sanitizer = new HtmlSanitizer();

            var captchaCode = HttpContext.Session.GetString("CaptchaCode");

            if (captchaCode == null || itemDto.Captcha != captchaCode)
            {
                ModelState.AddModelError("Captcha", "Неверный код CAPTCHA.");
            }

            if (!ModelState.IsValid)
            {
                return View(itemDto);
            }

            var user = await _userManager.GetUserAsync(User);
            var item = new Item
            {
                Name = sanitizer.Sanitize(itemDto.Name),
                Text = sanitizer.Sanitize(itemDto.Text),
                CreationDate = DateTime.UtcNow,
                UserId = user.Id
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("ShowAll", "Item");
        }

        [HttpGet]
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

            return View(item);
        }
    }
}