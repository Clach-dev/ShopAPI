using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderItemCases.Commands.DeleteOrderItemCase;

public record DeleteOrderItemCommand(
    Guid OrderItemId)
    : IRequest<Result<byte?>>;