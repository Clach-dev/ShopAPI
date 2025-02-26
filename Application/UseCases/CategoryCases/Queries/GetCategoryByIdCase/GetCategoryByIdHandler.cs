using Application.Common.Dtos.Category;
using Application.Common.Utils;
using AutoMapper;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetCategoriesByIdCase;

public class GetCategoryByIdHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetCategoryByIdQuery, Result<ReadCategoryDto>>
{
    public async Task<Result<ReadCategoryDto>> Handle(
        GetCategoryByIdQuery getCategoryByIdQuery,
        CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.Categories.GetByIdAsync(getCategoryByIdQuery.Id, cancellationToken);
        if (categories == null)
        {
            ResultBuilder.NotFoundResult<ReadCategoryDto>(ErrorMessages.CategoryIdNotFoundError);
        }
        
        var categoryDto = mapper.Map<ReadCategoryDto>(categories);
        
        return ResultBuilder.SuccessResult(categoryDto);
    }
}