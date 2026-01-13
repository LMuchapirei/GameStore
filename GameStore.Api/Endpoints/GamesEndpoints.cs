using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
   
    const string GetGameEndpointName = "GetGame"; // create list of games in memory   
    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");
        // GET all of the games in our database(in memory for now)
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            var gameList = await  dbContext.Games
            .Include(game=>game.Genre) /// tell ef to include related genre data
            .AsNoTracking() // improve performance for read-only queries
            .Select(game => new GameDto(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate
            )).ToListAsync();
            return gameList;
        });

        // GET /games/1
        group.MapGet("/{id}", async (int id,GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
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
        group.MapPost("/", async (CreateGameDto newGame,GameStoreContext dbContext) =>
        {
            Game game = new() 
            {
                Name =newGame.Name,
                Price =  newGame.Price,
                GenreId = newGame.GenreId,
                ReleaseDate = newGame.ReleaseDate,
            };
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();  
            GameDetailsDto gameDetailsDto = new (
                game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate
            );
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDetailsDto);
        });

        // PUT /game/id update existing game
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await  dbContext.Games.FindAsync(id);
            if (existingGame == null)
            {
                return Results.NotFound();
            }

           existingGame.Name = updatedGame.Name;
           existingGame.GenreId = updatedGame.GenreId;  
           existingGame.Price = updatedGame.Price;
           existingGame.ReleaseDate = updatedGame.ReleaseDate;
           await dbContext.SaveChangesAsync();
           return Results.NoContent();
        });

        // DELETE /game/id delete existing game
        group.MapDelete("/{id}", async (int id,GameStoreContext dbContext) =>
        { 
            await  dbContext.Games.Where(game=>game.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        });

    }

}



/// extension methods must be defined in a non-nested static class