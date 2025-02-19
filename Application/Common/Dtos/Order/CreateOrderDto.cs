namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for Order Create operation
/// </summary>
/// <param name="TotalPrice">double that contains TotalPrice of Order</param>
/// <param name="Status">string that contains Status of Order</param>
/// <param name="DeliveryDate">DateTime that contains DeliveryDate of Order</param>
/// <param name="UserId">Guid that contains Id of User</param>
/// <param name="OrderItemIds">IEnumerable_Guid that contains Ids of OrderItems</param>
public record CreateOrderDto(
    double TotalPrice,
    string Status,
    DateTime DeliveryDate,
    Guid UserId,
    IEnumerable<Guid> OrderItemIds);