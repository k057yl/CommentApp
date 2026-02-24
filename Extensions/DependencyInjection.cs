using CommentApp.Data;
using CommentApp.Services;
using CommentApp.Services.Interfaces;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommentApp.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

            services.AddCors(options => {
                options.AddPolicy("AngularPolicy", policy => {
                    policy.WithOrigins(allowedOrigins ?? new[] { "http://localhost:4200" })
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

            services.AddIdentityCore<IdentityUser>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddApiEndpoints();

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IFileService, FileService>();

            services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>(sp =>
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedTags.Add("a");
                sanitizer.AllowedTags.Add("code");
                sanitizer.AllowedTags.Add("i");
                sanitizer.AllowedTags.Add("strong");
                return sanitizer;
            });

            return services;
        }
    }
}
