using Application.Common.Dtos.OrderItem;
using Application.Common.Utils;
using AutoMapper;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.OrderItemCases.Queries.GetOrderItemByIdQuery;

public class GetOrderItemByIdHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) 
    : IRequestHandler<GetOrderItemByIdQuery, Result<ReadOrderItemDto>>
{
    public async Task<Result<ReadOrderItemDto>> Handle(
        GetOrderItemByIdQuery getOrderItemByIdQuery,
        CancellationToken cancellationToken)
    {
        var orderItem = await unitOfWork.OrderItems.GetByIdAsync(getOrderItemByIdQuery.OrderItemId, cancellationToken);
        if (orderItem is null)
        {
            return ResultBuilder.NotFoundResult<ReadOrderItemDto>(ErrorMessages.OrderItemIdNotFoundError);
        }
        
        var orderItemReadDto = mapper.Map<ReadOrderItemDto>(orderItem);
        return ResultBuilder.SuccessResult(orderItemReadDto);
    }
}