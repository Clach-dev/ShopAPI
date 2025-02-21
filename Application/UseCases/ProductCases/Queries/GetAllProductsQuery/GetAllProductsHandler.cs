using Application.Common.Dtos.Product;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;

namespace Application.UseCases.ProductCases.Queries.GetAllProductsQuery;

public class GetAllProductsHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
{
    public async Task<Result<ReadProductsDto>> Handle(
        GetAllProductsQuery getAllProductsQuery,
        CancellationToken cancellationToken)
    {
        var users = await unitOfWork.Products.GetAllAsync(
            mapper.Map<PageInfo>(getAllProductsQuery.PageInfoDto),
            cancellationToken);

        var usersReadDto = new ReadProductsDto(mapper.Map<IEnumerable<ReadProductDto>>(users.Item1), users.Item2);
        
        return ResultBuilder.SuccessResult(usersReadDto);
    }
}