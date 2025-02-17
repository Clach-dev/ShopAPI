namespace Application.Common.Dtos.Order;

public record UpdateOrderDto(
    Guid Id,
    bool TotalPrice,
    string? Status,
    DateTime? DeliveryDate,
    Guid? UserId,
    IEnumerable<Guid>? OrderItemIds);