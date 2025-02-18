namespace Application.Common.Dtos.OrderItem;

/// <summary>
/// Dto for OrderItem Delete operation
/// </summary>
/// <param name="Id">Guid that contains identifier of OrderItem</param>
public record DeleteOrderItemDto(
    Guid Id);