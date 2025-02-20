using Application.Common.Dtos.Order;
using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.OrderCases.Commands.CreateOrderCase;

public class CreateOrderHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateOrderCommand, Result<ReadOrderDto>>
{
    public async Task<Result<ReadOrderDto>> Handle(
        CreateOrderCommand createOrderCommand,
        CancellationToken cancellationToken)
    {
        var orderItems = (await unitOfWork.OrderItems.GetByPredicateAsync(orderItem => 
                createOrderCommand.OrderItemIds.Contains(orderItem.Id), new PageInfo(), cancellationToken))
            .ToList();
        if (orderItems.Count() != createOrderCommand.OrderItemIds.Count())
        {
            return ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.ProductIdNotFound);
        }
        
        var newOrder = mapper.Map<Order>(createOrderCommand);
        newOrder.OrderItems = orderItems;
        
        await unitOfWork.Orders.CreateAsync(newOrder, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var userReadDto = mapper.Map<ReadOrderDto>(newOrder);
        return ResultBuilder.CreatedResult(userReadDto);
    }
}