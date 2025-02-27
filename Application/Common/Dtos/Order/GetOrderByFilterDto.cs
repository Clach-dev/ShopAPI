namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for filtering orders
/// </summary>
/// <param name="Status">string that contains Status of Order</param>
/// <param name="DeliveryDate">DateTime that contains DeliveryDate of Order</param>
public record GetOrderByFilterDto(
    string? Status,
    DateTime? DeliveryDate,
    PageInfoDto PageInfoDto);