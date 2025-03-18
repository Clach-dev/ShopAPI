using System.Transactions;
using Application.Common.Utils;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.ProductCases.Commands.DeleteProductCase;

public class DeleteProductHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return ResultBuilder.NotFoundResult<Unit>(ErrorMessages.ProductIdNotFoundError);
        }

        
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            await unitOfWork.Products.Delete(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (product.ImageUri is not null)
            {
                await unitOfWork.ProductImages.DeleteFileAsync(product.ImageUri, cancellationToken);
            }

            transaction.Complete();
            return ResultBuilder.NoContentResult<Unit>();
        }
        catch (Exception)
        {
            return ResultBuilder.InternalServerErrorResult<Unit>(ErrorMessages.ProductDeletionFailureError);
        }
    }
}