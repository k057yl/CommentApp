using System.Drawing;
using System.Drawing.Imaging;

namespace CommentApp.Resources
{
    public class CaptchaService
    {
        // Теперь разрешены буквы в верхнем и нижнем регистре, а также цифры
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
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);

            // Рисуем текст CAPTCHA
            using var font = new Font("Arial", 24, FontStyle.Bold | FontStyle.Italic);
            using var brush = new SolidBrush(Color.Black);
            graphics.DrawString(captchaText, font, brush, new PointF(10, 10));

            // Добавляем шум
            for (int i = 0; i < 100; i++)
            {
                int x = _random.Next(width);
                int y = _random.Next(height);
                bitmap.SetPixel(x, y, Color.Gray);
            }

            // Сохраняем изображение в память
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }
    }
}