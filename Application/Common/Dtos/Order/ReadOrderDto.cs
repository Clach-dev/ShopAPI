namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for Order Read operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Order</param>
/// <param name="TotalPrice">double that contains TotalPrice of Order</param>
/// <param name="Status">string that contains Status of Order</param>
/// <param name="DeliveryDate">DateTime that contains DeliveryDate of Order</param>
public record ReadOrderDto(
    Guid Id,
    double TotalPrice,
    string Status,
    DateTime DeliveryDate);