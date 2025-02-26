using Application.Common.Utils;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.OrderItemCases.Commands.DeleteOrderItemCase;

public class DeleteOrderItemHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteOrderItemCommand, Result<byte?>>
{
    public async Task<Result<byte?>> Handle(
        DeleteOrderItemCommand deleteOrderItemCommand,
        CancellationToken cancellationToken)
    {
        var orderItem = await unitOfWork.OrderItems.GetByIdAsync(deleteOrderItemCommand.OrderItemId, cancellationToken);
        if (orderItem is null)
        {
            return ResultBuilder.NotFoundResult<byte?>(ErrorMessages.OrderItemIdNotFoundError);
        }
        
        await unitOfWork.OrderItems.Delete(orderItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return ResultBuilder.NoContentResult<byte?>();
    }
}