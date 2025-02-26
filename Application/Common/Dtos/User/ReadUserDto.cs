namespace Application.Common.Dtos.User;

/// <summary>
/// Dto for User Read operation
/// </summary>
/// <param name="Id">Guid that contains identifier of User</param>
/// <param name="LastName">string that contains LastName of User</param>
/// <param name="FirstName">string that contains FirstName of User</param>
/// <param name="MiddleName">string that contains MiddleName of User</param>
/// <param name="BirthDate">DateTime that contains BirthDate of User</param>
/// <param name="Email">string that contains Email of User</param>
public record ReadUserDto(
    Guid Id,
    string LastName, 
    string FirstName,
    string? MiddleName, 
    DateTime? BirthDate,
    string? Email);