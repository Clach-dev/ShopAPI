namespace Application.Common.Dtos;

/// <summary>
/// DTO for Pagination implementation
/// </summary>
/// <param name="PageNumber">Number of current page</param>
/// <param name="PageSize">Number of items per page</param>
public record PageInfoDto(
    int PageNumber = 1,
    int PageSize = 10);