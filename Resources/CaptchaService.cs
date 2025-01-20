using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Processing;

namespace CommentApp.Resources
{
    public class CaptchaService
    {
        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Random _random = new();

        /// <summary>
        /// Генерирует случайный текст для CAPTCHA.
        /// </summary>
        /// <param name="length">Длина текста</param>
        /// <returns>Случайный текст</returns>
        public string GenerateCaptchaText(int length = 6)
        {
            var captcha = new char[length];
            for (int i = 0; i < length; i++)
            {
                captcha[i] = AllowedChars[_random.Next(AllowedChars.Length)];
            }
            return new string(captcha);
        }

        /// <summary>
        /// Создаёт изображение CAPTCHA.
        /// </summary>
        /// <param name="captchaText">Текст CAPTCHA</param>
        /// <returns>Массив байтов изображения</returns>
        public byte[] GenerateCaptchaImage(string captchaText)
        {
            const int width = 200;
            const int height = 60;

            using var image = new Image<Rgba32>(width, height);

            image.Mutate(x => x.Fill(Color.White));

            Font font;

            try
            {
                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "ArialBlack.ttf");

                var fontCollection = new FontCollection();
                var family = fontCollection.Add(fontPath);
                font = family.CreateFont(24);
            }
            catch (Exception)
            {
                font = SystemFonts.CreateFont("Arial", 24);
            }

            image.Mutate(x => x.DrawText(captchaText, font, Color.Black, new PointF(10, 10)));

            for (int i = 0; i < 100; i++)
            {
                int x = _random.Next(width);
                int y = _random.Next(height);
                image[x, y] = Color.Gray;
            }

            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
            return memoryStream.ToArray();
        }
    }
}