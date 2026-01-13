using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record CreateGameDto
(
    [Required][StringLength(75)] string Name,
    [Required][Range(1,100)]int GenreId,
    [Range(1,10000)]decimal Price,
    DateOnly ReleaseDate
);