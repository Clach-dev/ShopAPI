using Application.Common.Dtos.Order;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderCases.Queries.GetOrderByIdCase;

public record GetOrderByIdQuery(
    Guid Id)
    : IRequest<Result<ReadOrderDto>>;