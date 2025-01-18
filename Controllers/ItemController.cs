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

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager, CaptchaService captchaService) : base()
        {
            _context = context;
            _userManager = userManager;
            _captchaService = captchaService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var captchaText = _captchaService.GenerateCaptchaText();
            HttpContext.Session.SetString("CaptchaCode", captchaText);

            var model = new ItemDTO();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemDTO model)
        {
            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (model.Captcha != sessionCaptcha)
            {
                ModelState.AddModelError("CaptchaInput", "Неверная CAPTCHA.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var item = new Item
                {
                    Name = model.Name,
                    Text = model.Text,
                    CreationDate = DateTime.UtcNow,
                    UserId = user.Id
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                return RedirectToAction("UserItems");
            }

            ModelState.AddModelError("", "Пользователь не найден.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserItems()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("UserItems", "Item");
            }

            var items = await _context.Items
                .Where(i => i.UserId == user.Id)
                .ToListAsync();

            return View(items);
        }
    }
}
