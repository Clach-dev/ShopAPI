using Application.Common.Dtos;
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
    : IRequestHandler<GetAllOrdersQuery, Result<IEnumerable<ReadOrderDto>>>
{
    public async Task<Result<IEnumerable<ReadOrderDto>>> Handle(
        GetAllOrdersQuery getAllOrdersQuery,
        CancellationToken cancellationToken)
    {
        var orders = await unitOfWork.Orders.GetAllAsync(mapper.Map<PageInfo>(getAllOrdersQuery), cancellationToken);

        var ordersReadDto = mapper.Map<IEnumerable<ReadOrderDto>>(orders);

        return ResultBuilder.SuccessResult(ordersReadDto); //
    }
}