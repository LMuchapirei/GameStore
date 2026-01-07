using System.Reflection.Metadata.Ecma335;
using GameStore.Api.Dtos;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// create list of games in memory   
List<GameDto> games =  [
    new GameDto(1, "F1 26", "Racing", 59.99m, new DateOnly(2024, 7, 11)),
    new GameDto(2, "Cyberpunk 2077", "RPG", 49.99m, new DateOnly(2020, 12, 10)),
    new GameDto(3, "The Witcher 3", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
    new GameDto(4, "Street Fighter", "Fighting", 39.99m, new DateOnly(2015, 5, 19))
];

app.MapGet("/", () => "Hello World! and fuck you nigga");

// GET all of the games in our database(in memory for now)
app.MapGet("/games", () =>
{
    return games;
});

app.Map("/games/{id}", (int id) =>
{
    return games.First(game=> game.Id == id);
});

app.Run();
