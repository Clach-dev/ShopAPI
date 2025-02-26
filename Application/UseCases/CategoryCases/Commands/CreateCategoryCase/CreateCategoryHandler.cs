using Application.Common.Dtos.Category;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Commands.CreateCategoryCase;

public class CreateCategoryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateCategoryCommand, Result<ReadCategoryDto>>
{
    public async Task<Result<ReadCategoryDto>> Handle(
        CreateCategoryCommand createCategoryCommand,
        CancellationToken cancellationToken)
    {
        var existingCategory = (await unitOfWork.Categories
            .GetByPredicateAsync(category => category.Name == createCategoryCommand.Name,
                new PageInfo(),
                cancellationToken)).Item1
            .FirstOrDefault();
        if (existingCategory is not null)
        {
            return ResultBuilder.ConflictResult<ReadCategoryDto>(ErrorMessages.ExistingCategoryError);
        }
        
        var newCategory = mapper.Map<Category>(createCategoryCommand);
        
        await unitOfWork.Categories.CreateAsync(newCategory, cancellationToken); 
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var categoryReadDto = mapper.Map<ReadCategoryDto>(newCategory);
        return ResultBuilder.CreatedResult(categoryReadDto);
    }
}
