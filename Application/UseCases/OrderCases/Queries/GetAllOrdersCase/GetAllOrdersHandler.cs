using Application.Common.Dtos.Order;
using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderCases.Queries.GetAllOrdersCase;

public class GetAllOrdersHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAllOrdersQuery, Result<ReadOrdersDto>>
{
    public async Task<Result<ReadOrdersDto>> Handle(
        GetAllOrdersQuery getAllOrdersQuery,
        CancellationToken cancellationToken)
    {
        var orders = await unitOfWork.Orders.GetAllAsync(mapper.Map<PageInfo>(getAllOrdersQuery.PageInfoDto), cancellationToken);
        
        var ordersReadDto = new ReadOrdersDto(mapper.Map<IEnumerable<ReadOrderDto>>(orders.Item1), orders.Item2);

        return ResultBuilder.SuccessResult(ordersReadDto);
    }
}