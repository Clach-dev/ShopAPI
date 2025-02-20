using Application.Common.Dtos.Order;
using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.UseCases.OrderCases.Queries.GetOrdersByFilterCase;

public class GetOrdersByFilterHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetOrdersByFilterQuery, Result<IEnumerable<ReadOrderDto>>>
{
public async Task<Result<IEnumerable<ReadOrderDto>>> Handle(
        GetOrdersByFilterQuery query,
        CancellationToken cancellationToken)
    {
        Expression<Func<Order, bool>> predicate = order =>
            (query.Status == null || order.Status.Contains(query.Status)) &&
            (query.DeliveryDate == null || order.DeliveryDate == query.DeliveryDate);

        var orders = await unitOfWork.Orders.GetByPredicateAsync(
            predicate, 
            mapper.Map<PageInfo>(query), 
            cancellationToken);

        return ResultBuilder.SuccessResult(mapper.Map<IEnumerable<ReadOrderDto>>(orders));
    }
}