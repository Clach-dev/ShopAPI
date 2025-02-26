namespace Application.Common.Dtos.Product;

/// <summary>
/// Dto for Products Read operation
/// </summary>
/// <param name="ReadProductDtos">IEnumerable_ReadProductDto that contains list of ReadProductDto</param>
/// <param name="TotalCount">int that contains total count of products</param>
public record ReadProductsDto(
    IEnumerable<ReadProductDto> ReadProductDtos,
    int TotalCount);