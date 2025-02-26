using Application.Common.Dtos.OrderItem;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderItemCases.Queries.GetOrderItemByIdQuery;

public record GetOrderItemByIdQuery(
    Guid OrderItemId)
    : IRequest<Result<ReadOrderItemDto>>;