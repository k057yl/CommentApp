using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommentApp.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager)
        {
            if (!await userManager.Users.AnyAsync())
            {
                var seedData = new List<(IdentityUser User, string Password)>
                {
                    (new IdentityUser { UserName = "admin", Email = "admin@test.com", EmailConfirmed = true }, "Qwe_123!"),
                    (new IdentityUser { UserName = "developer", Email = "dev@test.com", EmailConfirmed = true }, "Qwe_123!"),
                    (new IdentityUser { UserName = "tester", Email = "test@test.com", EmailConfirmed = true }, "Qwe_123!")
                };

                foreach (var (user, password) in seedData)
                {
                    var result = await userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to seed user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}