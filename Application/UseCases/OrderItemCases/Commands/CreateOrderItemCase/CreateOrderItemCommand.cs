using Application.Common.Dtos.OrderItem;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderItemCases.Commands.CreateOrderItemCase;

public record CreateOrderItemCommand(
    Guid ProductId,
    Guid OrderId,
    int Amount)
    : IRequest<Result<ReadOrderItemDto>>;