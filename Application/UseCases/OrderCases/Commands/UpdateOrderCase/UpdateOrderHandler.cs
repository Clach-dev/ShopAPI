using Application.Common.Dtos.Order;
using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderCases.Commands.UpdateOrderCase;

public class UpdateOrderHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) 
    : IRequestHandler<UpdateOrderCommand, Result<ReadOrderDto>>
{
    public async Task<Result<ReadOrderDto>> Handle(
        UpdateOrderCommand updateOrderCommand,
        CancellationToken cancellationToken)
    {
        if (updateOrderCommand.OrderItemIds == null) 
        {
            return ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.OrderItemIdNotFoundError);
        }
        
        var currentOrder = await unitOfWork.Orders.GetByIdAsync(updateOrderCommand.Id, cancellationToken);
        if (currentOrder is null)
        {
            return ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.OrderIdNotFound);
        }
        
        var orderItems = (await unitOfWork.OrderItems.GetByPredicateAsync(orderItem =>
                updateOrderCommand.OrderItemIds.Contains(orderItem.Id),
            new PageInfo(),
            cancellationToken)).Item1
            .ToList();
        if (orderItems.Count() != updateOrderCommand.OrderItemIds.Count())
        {
            return ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.ProductIdNotFound);
        }
        
        mapper.Map(updateOrderCommand, currentOrder);
        currentOrder.OrderItems = orderItems;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var orderReadDto = mapper.Map<ReadOrderDto>(currentOrder);
        return ResultBuilder.SuccessResult(orderReadDto);
    }
}