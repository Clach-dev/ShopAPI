namespace Application.Common.Dtos.User;

public record ReadUsersDto(
    IEnumerable<ReadUserDto> ReadUserDtos,
    int TotalCount);