namespace Application.Common.Dtos.User;

public record RegisterUserDto(
    string Login,
    string Email,
    string Password,
    string LastName,
    string FirstName,
    string? MiddleName,
    DateTime BirthDate);