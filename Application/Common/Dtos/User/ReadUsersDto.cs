namespace Application.Common.Dtos.User;

/// <summary>
/// Dto for Users Read operation
/// </summary>
/// <param name="ReadUserDtos">IEnumerable_ReadUserDto that contains list of ReadUserDto</param>
/// <param name="TotalCount">int that contains total count of users</param>
public record ReadUsersDto(
    IEnumerable<ReadUserDto> ReadUserDtos,
    int TotalCount);