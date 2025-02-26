namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for Order Create operation
/// </summary>
/// <param name="TotalPrice">decimal that contains TotalPrice of Order</param>
/// <param name="Status">string that contains Status of Order</param>
/// <param name="DeliveryDate">DateTime that contains DeliveryDate of Order</param>
/// <param name="UserId">Guid that contains UserId of User</param>
public record CreateOrderDto(
    decimal? TotalPrice,
    string Status,
    DateTime? DeliveryDate,
    Guid UserId);