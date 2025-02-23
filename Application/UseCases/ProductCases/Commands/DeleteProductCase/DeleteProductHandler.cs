using Application.Common.Utils;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.ProductCases.Commands.DeleteProductCase;

public class DeleteProductHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand, Result<byte?>>
{
    public async Task<Result<byte?>> Handle(
        DeleteProductCommand deleteProductCommand,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(deleteProductCommand.ProductId, cancellationToken);
        if (product is null)
        {
            return ResultBuilder.NotFoundResult<byte?>(ErrorMessages.ProductIdNotFoundError);
        }
        
        await unitOfWork.Products.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return ResultBuilder.NoContentResult<byte?>();
    }
}