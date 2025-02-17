namespace Application.Common.Dtos.Order;

public record CreateOrderDto(
    bool TotalPrice,
    string Status,
    DateTime DeliveryDate,
    Guid UserId,
    IEnumerable<Guid> OrderItemIds);