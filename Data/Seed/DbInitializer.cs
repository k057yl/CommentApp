using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommentApp.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager)
        {
            var hasUsers = await userManager.Users.AnyAsync<IdentityUser>();

            if (!hasUsers)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@test.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Qwe_123");
            }
        }
    }
}
