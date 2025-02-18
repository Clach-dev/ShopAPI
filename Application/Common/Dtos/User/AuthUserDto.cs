namespace Application.Common.Dtos.User;

/// <summary>
/// Dto for User Auth operation
/// </summary>
/// <param name="PhoneNumber">string that contains PhoneNumber of User</param>
/// <param name="Password">string that contains Password of User</param>
public record AuthUserDto(
    string PhoneNumber, 
    string Password);