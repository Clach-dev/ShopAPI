namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for Orders Read operation
/// </summary>
/// <param name="Orders">IEnumerable_ReadOrderDto that contains list of orders</param>
/// <param name="TotalCount">int that contains total count of orders</param>
public record ReadOrdersDto(
    IEnumerable<ReadOrderDto> Orders,
    int TotalCount);