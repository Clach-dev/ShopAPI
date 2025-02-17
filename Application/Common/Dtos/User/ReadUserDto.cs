namespace Application.Common.Dtos.User;

public record ReadUserDto(
    Guid Id,
    string Email,
    string LastName, 
    string FirstName,
    string PhoneNumber,     
    string? MiddleName, 
    DateTime BirthDate);