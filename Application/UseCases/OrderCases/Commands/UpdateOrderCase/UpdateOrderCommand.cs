using Application.Common.Dtos.Order;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderCases.Commands.UpdateOrderCase;

public record UpdateOrderCommand(
    Guid Id,
    double? TotalPrice,
    string? Status,
    DateTime? DeliveryDate,
    Guid? UserId,
    IEnumerable<Guid>? OrderItemIds)
    : IRequest<Result<ReadOrderDto>>;