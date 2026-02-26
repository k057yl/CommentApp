Полнофункциональная система древовидных комментариев с real-time обновлением, облачным хранилищем медиафайлов и серверной оптимизацией.

Стек технологий
Backend: .NET 9/10 (ASP.NET Core Web API)

Frontend: Angular 19 (Standalone components, Reactive Forms)

Database: PostgreSQL (Hosted on Render)

Cloud Storage: Cloudinary API (для изображений и текстовых файлов)

Real-time: SignalR (WebSockets)

Deployment: GitHub Pages (Client) & Render (API)

Ключевые особенности
Cloud Media Integration: Автоматическая загрузка, ресайз и оптимизация изображений (320x240) через ImageSharp перед отправкой в облако Cloudinary.

High-Performance Tree Building: Алгоритм сборки дерева комментариев на бэкенде с поддержкой бесконечной вложенности.

Smart Caching: Реализован IMemoryCache с логикой версионирования. Кэш инвалидируется автоматически при добавлении новых данных.

Advanced Security:

XSS Sanitization: Использование HtmlSanitizer для очистки пользовательского контента.

Server-side Captcha: Защита от спама на основе сессий.

Production Deployment: Настроена CORS-политика, сессии для кросс-доменных запросов и base-href для корректной работы SPA на GitHub Pages.

Приложение развернуто и доступно по адресу: https://k057yl.github.io/CommentApp/

Для локального запуска через Docker:

docker-compose up --build