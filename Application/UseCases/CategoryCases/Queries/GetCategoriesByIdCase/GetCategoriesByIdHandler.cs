using Application.Common.Dtos.Category;
using Application.Common.Utils;
using AutoMapper;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetCategoriesByIdCase;

public class GetCategoriesByIdHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetCategoriesByIdQuery, Result<ReadCategoryDto>>
{
    public async Task<Result<ReadCategoryDto>> Handle(
        GetCategoriesByIdQuery getCategoriesByIdQuery,
        CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.Categories.GetByIdAsync(getCategoriesByIdQuery.Id, cancellationToken);
        if (categories == null)
        {
            ResultBuilder.NotFoundResult<ReadCategoryDto>(ErrorMessages.CategoryIdNotFoundError);
        }
        
        var categoryDto = mapper.Map<ReadCategoryDto>(categories);
        
        return ResultBuilder.SuccessResult(categoryDto);
    }
}