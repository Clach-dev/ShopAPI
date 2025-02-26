using Application.Common.Dtos.Order;
using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderCases.Queries.GetOrdersByFilterCase;

public class GetOrdersByFilterHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetOrdersByFilterQuery, Result<ReadOrdersDto>>
{
public async Task<Result<ReadOrdersDto>> Handle(
        GetOrdersByFilterQuery getOrdersByFilterQuery,
        CancellationToken cancellationToken)
    {
        var orders = await unitOfWork.Orders.GetByPredicateAsync(order =>
                (getOrdersByFilterQuery.Status == null || order.Status.Contains(getOrdersByFilterQuery.Status)) &&
                (getOrdersByFilterQuery.DeliveryDate == null || order.DeliveryDate == getOrdersByFilterQuery.DeliveryDate), 
            mapper.Map<PageInfo>(getOrdersByFilterQuery.PageInfoDto), 
            cancellationToken);

        var ordersReadDto = new ReadOrdersDto(mapper.Map<IEnumerable<ReadOrderDto>>(orders.Item1), orders.Item2);
        
        return ResultBuilder.SuccessResult(ordersReadDto);
    }
}