using CommentApp.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CommentApp.Temp
{
    public class CaptchaController : Controller
    {
        private readonly CaptchaService _captchaService;

        public CaptchaController(CaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        [HttpGet]
        public IActionResult GenerateCaptcha()
        {
            var captchaText = _captchaService.GenerateCaptchaText();
            HttpContext.Session.SetString("CaptchaCode", captchaText);

            var captchaImage = _captchaService.GenerateCaptchaImage(captchaText);
            return File(captchaImage, "image/png");
        }
    }
}
