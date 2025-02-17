namespace Application.Common.Dtos.User;

public record ReadUserReducedDto(
    Guid Id,
    string LastName,
    string FirstName);