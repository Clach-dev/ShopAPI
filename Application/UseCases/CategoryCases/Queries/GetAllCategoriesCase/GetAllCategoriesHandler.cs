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
    : IRequestHandler<GetAllCategoriesQuery, Result<ReadCategoriesDto>>
{
    public async Task<Result<ReadCategoriesDto>> Handle(
        GetAllCategoriesQuery getAllCategoriesQuery,
        CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.Categories
            .GetAllAsync(mapper.Map<PageInfo>(getAllCategoriesQuery.PageInfoDto), cancellationToken);
        
        var categoriesDto = new ReadCategoriesDto(mapper.Map<IEnumerable<ReadCategoryDto>>(categories.Item1), categories.Item2);

        return ResultBuilder.SuccessResult(categoriesDto);
    }
}