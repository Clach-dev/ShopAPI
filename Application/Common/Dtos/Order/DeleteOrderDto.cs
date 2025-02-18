namespace Application.Common.Dtos.Order;

/// <summary>
/// Dto for Order Delete operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Order</param>
public record DeleteOrderDto(
    Guid Id);