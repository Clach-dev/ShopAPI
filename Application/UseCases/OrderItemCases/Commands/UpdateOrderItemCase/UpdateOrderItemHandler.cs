using Application.Common.Dtos.OrderItem;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.OrderItemCases.Commands.UpdateOrderItemCase;

public class UpdateOrderItemHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) 
    : IRequestHandler<UpdateOrderItemCommand, Result<ReadOrderItemDto>>
{
    public async Task<Result<ReadOrderItemDto>> Handle(
        UpdateOrderItemCommand updateOrderItemCommand,
        CancellationToken cancellationToken)
    {
        var currentOrderItem = await unitOfWork.OrderItems.GetByIdAsync(updateOrderItemCommand.Id, cancellationToken);
        if (currentOrderItem is null)
        {
            return ResultBuilder.NotFoundResult<ReadOrderItemDto>(ErrorMessages.OrderItemIdNotFoundError);
        }
    
        var existingOrderItem = (await unitOfWork
                .OrderItems
                .GetByPredicateAsync(orderItem => orderItem.ProductId == updateOrderItemCommand.ProductId &&
                                                  orderItem.OrderId == updateOrderItemCommand.OrderId &&
                                                  orderItem.Amount == updateOrderItemCommand.Amount,
                    new PageInfo(),
                    cancellationToken)).Item1
            .FirstOrDefault();
        if (existingOrderItem is not null)
        {
            return ResultBuilder.ConflictResult<ReadOrderItemDto>(ErrorMessages.ExistingOrderItemError);
        }
    
        mapper.Map(updateOrderItemCommand, currentOrderItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    
        var orderItemReadDto = mapper.Map<ReadOrderItemDto>(currentOrderItem);
        return ResultBuilder.SuccessResult(orderItemReadDto);
    }
}