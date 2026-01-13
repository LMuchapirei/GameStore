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
