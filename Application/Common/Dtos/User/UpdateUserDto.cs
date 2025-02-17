namespace Application.Common.Dtos.User;

public record UpdateUserDto(
    string? Login,
    string? Password,
    string? Email,
    string? LastName,
    string? FirstName,
    string? MiddleName,
    DateTime? BirthDate);