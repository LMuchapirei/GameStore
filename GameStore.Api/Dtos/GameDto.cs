namespace GameStore.Api.Dtos;

// A DTO is a contract between the client and server since it represents a shared
// agreement about how data will be transffered and user.
public record  GameDto
(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate
);
