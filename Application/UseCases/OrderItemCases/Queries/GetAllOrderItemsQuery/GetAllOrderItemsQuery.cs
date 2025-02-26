using Application.Common.Dtos;
using Application.Common.Dtos.OrderItem;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderItemCases.Queries.GetAllOrderItemsQuery;

public record GetAllOrderItemsQuery(
    PageInfoDto PageInfoDto)
    : IRequest<Result<ReadOrderItemsDto>>;