using Application.Common.Dtos;
using Application.Common.Dtos.Order;
using Application.Common.Utils;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderCases.Queries.GetAllOrdersCase;

public record GetAllOrdersQuery(
    PageInfoDto PageInfoDto)
    : IRequest<Result<IEnumerable<ReadOrderDto>>>;