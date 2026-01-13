using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record UpdateGameDto
(
    [Required][StringLength(75)] string Name,
    [Required][StringLength(50)] string Genre,
    [Range(1,10000)]decimal Price,
    DateOnly ReleaseDate
);