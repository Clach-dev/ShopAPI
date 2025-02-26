using Application.Common.Dtos.OrderItem;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.OrderItemCases.Commands.CreateOrderItemCase;

public class CreateOrderItemHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateOrderItemCommand, Result<ReadOrderItemDto>>
{
    public async Task<Result<ReadOrderItemDto>> Handle(
        CreateOrderItemCommand createOrderItemCommand,
        CancellationToken cancellationToken)
    {
        var existingOrderItem = (await unitOfWork
                .OrderItems
                .GetByPredicateAsync(orderItem =>
                        orderItem.Amount == createOrderItemCommand.Amount &&
                        orderItem.ProductId == createOrderItemCommand.ProductId &&
                        orderItem.OrderId.CompareTo(createOrderItemCommand.OrderId) == 0 ,
                    new PageInfo(),
                    cancellationToken)).Item1
            .FirstOrDefault();
        if (existingOrderItem is not null)
        {
            return ResultBuilder.ConflictResult<ReadOrderItemDto>(ErrorMessages.ExistingOrderItemError);
        }

        if (await unitOfWork.Orders.GetByIdAsync(createOrderItemCommand.OrderId, cancellationToken) is null ||
            await unitOfWork.Products.GetByIdAsync(createOrderItemCommand.ProductId, cancellationToken) is null)
        {
            return ResultBuilder.NotFoundResult<ReadOrderItemDto>(ErrorMessages.OrderItemDataNotFoundError);
        }
        
        var newOrderItem = mapper.Map<OrderItem>(createOrderItemCommand);

        await unitOfWork.OrderItems.CreateAsync(newOrderItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var orderItemReadDto = mapper.Map<ReadOrderItemDto>(newOrderItem);
        return ResultBuilder.CreatedResult(orderItemReadDto);
    }
}