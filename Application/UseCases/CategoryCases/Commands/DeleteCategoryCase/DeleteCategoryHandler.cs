using Application.Common.Utils;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Commands.DeleteCategoryCase;

public class DeleteCategoryHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCategoryCommand, Result<Unit>>
{

    public async Task<Result<Unit>> Handle(
        DeleteCategoryCommand deleteCategoryCommand,
        CancellationToken cancellationToken)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(deleteCategoryCommand.Id, cancellationToken);
        if (category is null)
        {
            return ResultBuilder.NotFoundResult<Unit>(ErrorMessages.CategoryIdNotFoundError);
        }
        
        await unitOfWork.Categories.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return ResultBuilder.NoContentResult<Unit>();
    }
}