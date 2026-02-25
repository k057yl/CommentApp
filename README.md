Полнофункциональная система древовидных комментариев с real-time обновлением и серверной оптимизацией.

Стек технологий 
Backend: .NET 10 (Bleeding Edge) 
Frontend: Angular 18 (Standalone components, Reactive Forms) 
Database: PostgreSQL 15 Real-time: SignalR (WebSockets) 
Infrastructure: Docker & Docker Compose

Ключевые особенности

High-Performance Tree Building: Алгоритм сборки дерева комментариев на бэкенде.
Smart Caching & Versioning: Реализован IMemoryCache с логикой инвалидации. Кэш сбрасывается мгновенно при добавлении нового комментария через SignalR,
гарантируя актуальность данных.
Advanced Security: XSS Sanitization: Используется HtmlSanitizer для очистки пользовательского ввода. Server-side Captcha: Защита от спама через сессии.
Resilient Infrastructure: Настроена Retry-логика подключения к БД в Docker, что гарантирует успешный запуск всей системы одной командой.

Быстрый запуск Убедитесь, что у вас запущен Docker Desktop, и выполните команду в корне проекта:

docker-compose up --build

Приложение будет доступно по адресу: http://localhost:4200