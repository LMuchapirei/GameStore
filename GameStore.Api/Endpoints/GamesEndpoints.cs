using System;
using GameStore.Api.Dtos;

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
        group.MapGet("/", () =>
        {
            return games;
        });

        // GET /games/1
        group.Map("/{id}", (int id) =>
        {
            var game = games.Find(game=> game.Id == id);
            return game is not null ? Results.Ok(game) : Results.NotFound();
        })
        .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto gameToAdd = new GameDto
            (
                Id: games.Max(g => g.Id) + 1,
                Name: newGame.Name,
                Genre: newGame.Genre,
                Price: newGame.Price,
                ReleaseDate: newGame.ReleaseDate
            );
            games.Add(gameToAdd);    
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameToAdd.Id }, gameToAdd);
        });

        // PUT /game/id update existing game
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var existingGameIndex = games.FindIndex(g => g.Id == id);
            if (existingGameIndex == -1)
            {
                return Results.NotFound();
            }

            GameDto gameToUpdate = new GameDto
            (
                Id: id,
                Name: updatedGame.Name,
                Genre: updatedGame.Genre,
                Price: updatedGame.Price,
                ReleaseDate: updatedGame.ReleaseDate
            );

            games[existingGameIndex] = gameToUpdate;
            return Results.NoContent();
        });

        // DELETE /game/id delete existing game
        app.MapDelete("/{id}", (int id) =>
        { 
            games.RemoveAll(g => g.Id == id);
            return Results.NoContent();
        });

    }

}



/// extension methods must be defined in a non-nested static class