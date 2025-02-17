namespace Application.Common.Dtos.Order;

public record ReadOrderDto(
    bool TotalPrice,
    string Status,
    DateTime DeliveryDate,
    Guid UserId,
    IEnumerable<Guid> OrderItemIds);