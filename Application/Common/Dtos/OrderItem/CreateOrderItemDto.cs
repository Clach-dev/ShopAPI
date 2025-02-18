namespace Application.Common.Dtos.OrderItem;

/// <summary>
/// Dto for OrderItem Create operation
/// </summary>
/// <param name="ProductId">Guid that contains identifier of Product</param>
/// <param name="OrderId">Guid that contains identifier of Order</param>
/// <param name="Amount">int that contains Amount of OrderItem</param>
public record CreateOrderItemDto(
    Guid ProductId,
    Guid OrderId,
    int Amount);