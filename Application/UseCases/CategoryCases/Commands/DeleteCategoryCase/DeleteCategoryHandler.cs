using System.Reflection.Metadata;
using Application.Common.Utils;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Commands.DeleteCategoryCase;

public class DeleteCategoryHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCategoryCommand, Result<byte?>>
{

    public async Task<Result<byte?>> Handle(
        DeleteCategoryCommand deleteCategoryCommand,
        CancellationToken cancellationToken)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(deleteCategoryCommand.Id, cancellationToken);
        if (category == null)
        {
            return ResultBuilder.NotFoundResult<byte?>(ErrorMessages.CategoryNotFoundError);
        }
        
        await unitOfWork.Categories.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return ResultBuilder.NoContentResult<byte?>();
    }
}