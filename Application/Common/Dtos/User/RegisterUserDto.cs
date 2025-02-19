namespace Application.Common.Dtos.User;

/// <summary>
/// Dto for User Register operation
/// </summary>
/// <param name="PhoneNumber">string that contains PhoneNumber of User</param>
/// <param name="Password">string that contains Password of User</param>
/// <param name="LastName">string that contains LastName of User</param>
/// <param name="FirstName">string that contains FirstName of User</param>
/// <param name="MiddleName">string that contains MiddleName of User</param>
/// <param name="BirthDate">DateTime that contains BirthDate of User</param>
/// <param name="Email">string that contains Email of User</param>
public record RegisterUserDto(
    string PhoneNumber,
    string Password,
    string LastName,
    string FirstName,
    string? MiddleName,
    DateTime? BirthDate,
    string? Email);