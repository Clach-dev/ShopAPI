using Application.Common.Dtos.Order;
using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using AutoMapper;
using MediatR;

namespace Application.UseCases.OrderCases.Queries.GetOrderByIdCase;

public class GetOrderByIdHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetOrderByIdQuery, Result<ReadOrderDto>>
{
    public async Task<Result<ReadOrderDto>> Handle(
        GetOrderByIdQuery getOrderByIdQuery,
        CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(getOrderByIdQuery.Id, cancellationToken);
        if (order is null)
        {
            ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.OrderIdNotFoundError);
        }
        
        var orderReadDto = mapper.Map<ReadOrderDto>(order);

        return ResultBuilder.SuccessResult(orderReadDto);
    }
}