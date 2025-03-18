using Domain.Interfaces.IRepositories;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderCases.Commands.DeleteOrderCase;

public class DeleteOrderHandler(
    IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteOrderCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        DeleteOrderCommand deleteOrderCommand,
        CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(deleteOrderCommand.Id, cancellationToken);
        if (order is null)
        {
            return ResultBuilder.NotFoundResult<Unit>(ErrorMessages.OrderIdNotFoundError);
        }
        
        await unitOfWork.Orders.Delete(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return ResultBuilder.NoContentResult<Unit>();
    }
}