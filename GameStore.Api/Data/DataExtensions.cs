using System;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static  class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }

    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("GameStore");
        // DbContext has a Scoped service lifetime because
        // 1. It ensures that a new instance of the DbContext is created for each web request, which helps to prevent issues related to shared state and concurrency.
        // 2. DB connections are a limited and expensive resource and using a scoped lifetime helps to manage these resources efficiently by ensuring that they are released at the end of each request.
        // 3. DbContext is not thread-safe, and using a scoped lifetime helps to avoid potential threading issues by ensuring that each request has its own instance.
        // 4. Makes it easier to manage transactions and ensure data consistency within the scope of a single web request.
        // 5. Reusing a DbContext instance can lead to increased memory usage and potential memory leaks, especially in long-running applications. Using a scoped lifetime helps to mitigate these risks by ensuring that DbContext instances are disposed of at the end of each request.
        builder.Services.AddSqlite<GameStoreContext>(
            connString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if(!context.Set<Genre>().Any())
                {
                    var action = new Genre { Name = "Action" };
                    var rpg = new Genre { Name = "RPG" };
                    var strategy = new Genre { Name = "Strategy" };
                    var racing = new Genre { Name = "Racing" };
                    var sports = new Genre { Name = "Sports" };
                    context.Set<Genre>().AddRange(action, rpg, strategy, racing,sports);
                    context.SaveChanges();
                }
            })
            );
    }
}
