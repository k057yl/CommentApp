using CommentApp.Hubs;
using CommentApp.Models.DTOs;
using CommentApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CommentApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IFileService _fileService;
        private readonly IHubContext<CommentHub> _hubContext;

        public CommentsController(ICommentService commentService, IFileService fileService, IHubContext<CommentHub> hubContext)
        {
            _commentService = commentService;
            _fileService = fileService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 25, 
            [FromQuery] string sortBy = "date", [FromQuery] string sortOrder = "desc")
        {
            var (items, totalCount) = await _commentService.GetCommentsPagedAsync(page, pageSize, sortBy, sortOrder);

            return Ok(new
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            });
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCommentRequest request)
        {
            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(sessionCaptcha) || request.Captcha != sessionCaptcha)
            {
                return BadRequest(new { message = "Неверный код капчи" });
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? imagePath = null;
            string? textFilePath = null;

            if (request.Image != null)
            {
                imagePath = await _fileService.SaveImageAsync(request.Image);
                if (imagePath == null) return BadRequest(new { message = "Ошибка загрузки или неверный формат изображения" });
            }

            if (request.TextFile != null)
            {
                textFilePath = await _fileService.SaveTextFileAsync(request.TextFile);
                if (textFilePath == null) return BadRequest(new { message = "Текстовый файл должен быть .txt и не более 100Кб" });
            }

            var result = await _commentService.CreateCommentAsync(request, imagePath, textFilePath);

            await _hubContext.Clients.All.SendAsync("ReceiveComment");

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet("captcha")]
        public IActionResult GetCaptcha()
        {
            var code = new Random().Next(1000, 9999).ToString();
            HttpContext.Session.SetString("CaptchaCode", code);

            return Ok(new { captchaId = Guid.NewGuid(), code = code });
        }
    }
}
