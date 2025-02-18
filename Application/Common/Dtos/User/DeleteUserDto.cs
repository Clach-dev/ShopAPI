namespace Application.Common.Dtos.User;

/// <summary>
/// Dto for User Delete operation
/// </summary>
/// <param name="Id">Guid that contains identifier of User</param>
public record DeleteUserDto(
    Guid Id);