using Application.Common.Dtos.Category;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetCategoriesByName;

public class GetCategoriesHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetCategoriesByNameQuery, Result<IEnumerable<ReadCategoryDto>>>
{
    public async Task<Result<IEnumerable<ReadCategoryDto>>> Handle(
        GetCategoriesByNameQuery getCategoriesByNameQuery,
        CancellationToken cancellationToken)
    {
        var categories = (await unitOfWork
            .Categories
            .GetByPredicateAsync(category => category.Name.Contains(getCategoriesByNameQuery.Name), new PageInfo(), cancellationToken));
        
        var categoriesDto = mapper.Map<IEnumerable<ReadCategoryDto>>(categories);

        return ResultBuilder.SuccessResult(categoriesDto);
    }
}