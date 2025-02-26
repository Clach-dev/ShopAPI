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
        var user = await unitOfWork.Users.GetByIdAsync(createOrderCommand.UserId, cancellationToken);
        if (user == null)
        {
            return ResultBuilder.NotFoundResult<ReadOrderDto>(ErrorMessages.UserIdNotFoundError);
        }
        
        var newOrder = mapper.Map<Order>(createOrderCommand);
        
        await unitOfWork.Orders.CreateAsync(newOrder, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var userReadDto = mapper.Map<ReadOrderDto>(newOrder);
        return ResultBuilder.CreatedResult(userReadDto);
    }
}