namespace Application.Common.Dtos.Product;

/// <summary>
/// Dto for filtering products operation
/// </summary>
/// <param name="Name">string that contains Name of Product</param>
/// <param name="MinPrice">decimal that contains MinPrice of Product</param>
/// <param name="MaxPrice">decimal that contains MaxPrice of Product</param>
/// <param name="CategoryIds">IEnumerable_Guid that contains Ids of Categories</param>
/// <param name="PageInfoDto">PageInfoDto that contains PageNumber and PageSize</param>
public record GetProductsByFilterDto(
    string? Name,
    decimal? MinPrice,
    decimal? MaxPrice,
    IEnumerable<Guid>? CategoryIds,
    PageInfoDto PageInfoDto);
