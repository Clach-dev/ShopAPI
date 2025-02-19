namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for Order Update operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Order</param>
/// <param name="TotalPrice">double that contains Id of User</param>
/// <param name="Status">string that contains Status of Order</param>
/// <param name="DeliveryDate">DaetTime that contains DeliveryDate of Order</param>
/// <param name="UserId">Guid that contains Id of User</param>
/// <param name="OrderItemIds">IEnumerable_Guid that contains Ids of OrderItems</param>
public record UpdateOrderDto(
    Guid Id,
    double? TotalPrice,
    string? Status,
    DateTime? DeliveryDate,
    Guid? UserId,
    IEnumerable<Guid>? OrderItemIds);