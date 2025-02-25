using Application.Common.Dtos.Order;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderCases.Commands.CreateOrderCase;

public record CreateOrderCommand(
    decimal? TotalPrice,
    string Status,
    DateTime? DeliveryDate,
    Guid UserId,
    IEnumerable<Guid>? OrderItemIds)
    : IRequest<Result<ReadOrderDto>>;