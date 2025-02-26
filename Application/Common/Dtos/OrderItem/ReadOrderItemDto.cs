namespace Application.Common.Dtos.OrderItem;

/// <summary>
/// Dto for OrderItem Read operation
/// </summary>
/// <param name="Id">Guid that contains identifier of OrderItem</param>
/// <param name="ProductId">Guid that contains identifier of Product</param>
/// <param name="OrderId">Guid that contains identifier of Order</param>
/// <param name="Amount">int that contains Amount of items in order</param>
public record ReadOrderItemDto(
    Guid Id,
    Guid ProductId,
    Guid OrderId,
    int Amount);