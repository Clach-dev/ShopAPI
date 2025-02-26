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
        var currentOrder = await unitOfWork.Orders.GetByIdAsync(updateOrderCommand.Id, cancellationToken);
        if (currentOrder is null)
        {
            return ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.OrderIdNotFoundError);
        }

        if (updateOrderCommand.UserId != null)
        {
            var user = await unitOfWork.Users.GetByIdAsync(updateOrderCommand.UserId.Value, cancellationToken);
            if (user == null)
            {
                return ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.UserIdNotFoundError);
            }
        }
        
        mapper.Map(updateOrderCommand, currentOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var orderReadDto = mapper.Map<ReadOrderDto>(currentOrder);
        return ResultBuilder.SuccessResult(orderReadDto);
    }
}