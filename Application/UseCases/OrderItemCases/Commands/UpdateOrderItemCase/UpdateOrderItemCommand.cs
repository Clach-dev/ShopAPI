using Application.Common.Dtos.OrderItem;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderItemCases.Commands.UpdateOrderItemCase;

public record UpdateOrderItemCommand(
    Guid Id,
    Guid? ProductId,
    Guid? OrderId,
    int? Amount)
    : IRequest<Result<ReadOrderItemDto>>;