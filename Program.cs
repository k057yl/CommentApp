using CommentApp.Data;
using CommentApp.Data.Seed;
using CommentApp.Extensions;
using CommentApp.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBusinessServices();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    for (int i = 0; i < 5; i++)
    {
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await DbInitializer.SeedAsync(userManager);

            logger.LogInformation("----> Database initialized and seeded successfully!");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Попытка {i + 1} не удалась. База еще спит... Ждем.");
            if (i == 4) logger.LogError(ex, "Не удалось подключиться к базе после 5 попыток.");
            await Task.Delay(3000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseResponseCaching();

app.UseCors("AngularPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.MapHub<CommentHub>("/commentHub");


app.Run();