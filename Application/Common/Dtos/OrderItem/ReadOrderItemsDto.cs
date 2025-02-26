namespace Application.Common.Dtos.OrderItem;

/// <summary>
/// Dto for Products Read operation
/// </summary>
/// <param name="ReadOrderItemDtos">IEnumerable_ReadOrderItemDto that contains list of ReadOrderItemDto</param>
/// <param name="TotalCount">int that contains total count of products</param>
public record ReadOrderItemsDto(
    IEnumerable<ReadOrderItemDto> ReadOrderItemDtos,
    int TotalCount);