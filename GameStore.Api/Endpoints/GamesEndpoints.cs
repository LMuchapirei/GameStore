using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
   
    const string GetGameEndpointName = "GetGame"; // create list of games in memory   
    private static readonly  List<GameDto> games =  [
        new GameDto(1, "F1 26", "Racing", 59.99m, new DateOnly(2024, 7, 11)),
        new GameDto(2, "Cyberpunk 2077", "RPG", 49.99m, new DateOnly(2020, 12, 10)),
        new GameDto(3, "The Witcher 3", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new GameDto(4, "Street Fighter", "Fighting", 39.99m, new DateOnly(2015, 5, 19))
    ];
    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");
        // GET all of the games in our database(in memory for now)
        group.MapGet("/", (GameStoreContext dbContext) =>
        {
            var gameList = dbContext.Games.Select(game => new GameDetailsDto(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            )).ToList();
            return gameList;
        });

        // GET /games/1
        group.Map("/{id}", (int id,GameStoreContext dbContext) =>
        {
            var game = dbContext.Games.FirstOrDefault(x => x.Id == id);
            if(game == null)    
            {
                return Results.NotFound();
            }
            var gameDetailsDto = new GameDetailsDto(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            );
            return Results.Ok(gameDetailsDto);
            
        })
        .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame,GameStoreContext dbContext) =>
        {
            Game game = new() 
            {
                Name =newGame.Name,
                Price =  newGame.Price,
                GenreId = newGame.GenreId,
                ReleaseDate = newGame.ReleaseDate,
            };
            dbContext.Games.Add(game);
            dbContext.SaveChanges();  
            GameDetailsDto gameDetailsDto = new (
                game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate
            );
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDetailsDto);
        });

        // PUT /game/id update existing game
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = dbContext.Games.FirstOrDefault(g => g.Id == id);
            if (existingGame == null)
            {
                return Results.NotFound();
            }

           existingGame.Name = updatedGame.Name;
           existingGame.GenreId = updatedGame.GenreId;  
           existingGame.Price = updatedGame.Price;
           existingGame.ReleaseDate = updatedGame.ReleaseDate;
           dbContext.SaveChanges();
           return Results.NoContent();
        });

        // DELETE /game/id delete existing game
        app.MapDelete("/{id}", (int id,GameStoreContext dbContext) =>
        { 
            var game = dbContext.Games.First(g => g.Id == id);
            if (game == null)
            {
                return Results.NotFound();
            }
            dbContext.Games.Remove(game);
            dbContext.SaveChanges(); 
            return Results.NoContent();
        });

    }

}



/// extension methods must be defined in a non-nested static class