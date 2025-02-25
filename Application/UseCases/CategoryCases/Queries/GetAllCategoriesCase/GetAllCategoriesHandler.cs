using Application.Common.Dtos.Category;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetAllCategoriesCase;

public class GetAllCategoriesHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<ReadCategoryDto>>>
{
    public async Task<Result<IEnumerable<ReadCategoryDto>>> Handle(
        GetAllCategoriesQuery getAllCategoriesQuery,
        CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.Categories.GetAllAsync(mapper.Map<PageInfo>(getAllCategoriesQuery), cancellationToken);
        
        var categoriesDto = mapper.Map<IEnumerable<ReadCategoryDto>>(categories);

        return ResultBuilder.SuccessResult(categoriesDto);
    }
}