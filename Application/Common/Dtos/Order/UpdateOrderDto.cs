namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for Order Update operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Order</param>
/// <param name="TotalPrice">decimal that contains TotalPrice of Order</param>
/// <param name="Status">string that contains Status of Order</param>
/// <param name="DeliveryDate">DateTime that contains DeliveryDate of Order</param>
/// <param name="UserId">Guid that contains UserId of User</param>
public record UpdateOrderDto(
    Guid Id,
    decimal? TotalPrice,
    string? Status,
    DateTime? DeliveryDate,
    Guid? UserId);