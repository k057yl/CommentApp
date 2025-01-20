using CommentApp.Data;
using CommentApp.Models.DTOs;
using CommentApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ganss.Xss;
using CommentApp.Resources;

namespace CommentApp.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HtmlValidator _htmlValidator;

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager, HtmlValidator htmlValidator)
        {
            _context = context;
            _userManager = userManager;
            _htmlValidator = htmlValidator;
        }

        public IActionResult ShowAll(string sortOrder, int page = 1)
        {
            ViewData["UserNameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "user_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            const int pageSize = 25;

            var items = _context.Items
                .Include(i => i.User)
                .Include(i => i.Comments)
                    .ThenInclude(c => c.User)
                .AsQueryable();

            foreach (var item in items)
            {
                if (sortOrder == "user_desc")
                {
                    item.Comments = item.Comments.OrderByDescending(c => c.User.UserName).ToList();
                }
                else if (sortOrder == "Date")
                {
                    item.Comments = item.Comments.OrderBy(c => c.CreationDate).ToList();
                }
                else if (sortOrder == "date_desc")
                {
                    item.Comments = item.Comments.OrderByDescending(c => c.CreationDate).ToList();
                }
                else
                {
                    item.Comments = item.Comments.OrderBy(c => c.User.UserName).ToList();
                }
            }

            var totalItems = items.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedItems = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(paginatedItems);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ItemDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemDTO itemDto, string Captcha)
        {
            if (!_htmlValidator.ValidateHtml(itemDto.Name) || !_htmlValidator.ValidateHtml(itemDto.Text))
            {
                ModelState.AddModelError("", "Сообщение содержит недопустимые теги или атрибуты.");
                return View(itemDto);
            }

            var sanitizer = new HtmlSanitizer();

            itemDto.Name = sanitizer.Sanitize(itemDto.Name);
            itemDto.Text = sanitizer.Sanitize(itemDto.Text);

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
                Name = itemDto.Name,
                Text = itemDto.Text,
                CreationDate = DateTime.UtcNow,
                UserId = user.Id
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("ShowAll", "Item");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string sortOrder)
        {
            ViewData["UserNameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "user_name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var item = await _context.Items
                .Include(i => i.User)
                .Include(i => i.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

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

            return View(item);
        }
    }
}