using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderCases.Commands.DeleteOrderCase;

public class DeleteOrderHandler(
    IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteOrderCommand, Result<byte?>>
{
    public async Task<Result<byte?>> Handle(
        DeleteOrderCommand deleteOrderCommand,
        CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(deleteOrderCommand.Id, cancellationToken);
        if (order is null)
        {
            return ResultBuilder.NotFoundResult<byte?>(ErrorMessages.OrderIdNotFound);
        }
        
        await unitOfWork.Orders.Delete(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return ResultBuilder.NoContentResult<byte?>();
    }
}