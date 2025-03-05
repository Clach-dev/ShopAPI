using Application.Common.Dtos.Category;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Commands.UpdateCategoryCase;

public class UpdateCategoryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<UpdateCategoryCommand, Result<ReadCategoryDto>>
{
    public async Task<Result<ReadCategoryDto>> Handle(
        UpdateCategoryCommand updateCategoryCommand,
        CancellationToken cancellationToken)
    {
        var currentCategory = await unitOfWork.Categories.GetByIdAsync(updateCategoryCommand.Id, cancellationToken);
        if (currentCategory is null)
        {
            return ResultBuilder.NotFoundResult<ReadCategoryDto>(ErrorMessages.CategoryIdNotFoundError);
        }

        var category = (await unitOfWork
                .Categories
                .GetByPredicateAsync(category => category.Name == updateCategoryCommand.Name, new PageInfo(), cancellationToken)).Item1
            .FirstOrDefault();
        if (category is not null)
        {
            return ResultBuilder.ConflictResult<ReadCategoryDto>(ErrorMessages.ExistingCategoryError);
        }
        
        mapper.Map(updateCategoryCommand, currentCategory);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var readCategoryDto = mapper.Map<ReadCategoryDto>(currentCategory);
        return ResultBuilder.SuccessResult(readCategoryDto);
    }
}