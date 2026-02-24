using CommentApp.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CommentApp.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public FileService(IWebHostEnvironment env)
        {
            _env = env;

            var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }

        public async Task<string?> SaveImageAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedImageExtensions.Contains(ext)) return null;

            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(_env.WebRootPath, "uploads", "images");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var fullPath = Path.Combine(path, fileName);

            using var image = await Image.LoadAsync(file.OpenReadStream());

            if (image.Width > 320 || image.Height > 240)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(320, 240),
                    Mode = ResizeMode.Max
                }));
            }

            await image.SaveAsync(fullPath);
            return $"/uploads/images/{fileName}";
        }

        public async Task<string?> SaveTextFileAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".txt" || file.Length > 100 * 1024) return null;

            var fileName = $"{Guid.NewGuid()}.txt";
            var path = Path.Combine(_env.WebRootPath, "uploads", "docs");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var fullPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/docs/{fileName}";
        }
    }
}
