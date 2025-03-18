using Application.Common.Dtos.Product;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.ProductCases.Queries.GetAllProductsQuery;

public class GetAllProductsHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAllProductsQuery, Result<ReadProductsDto>>
{
    public async Task<Result<ReadProductsDto>> Handle(
        GetAllProductsQuery getAllProductsQuery,
        CancellationToken cancellationToken)
    {
        var products = await unitOfWork.Products.GetAllAsync(
            mapper.Map<PageInfo>(getAllProductsQuery.PageInfoDto),
            cancellationToken);
        
        foreach (var product in products.Item1)
        {
            if (product.ImageUri != null)
            {
                product.ImageUri = unitOfWork.ProductImages.GetReadOnlyImageUri(product.ImageUri);
            }
        }

        var productsReadDto = new ReadProductsDto(mapper.Map<IEnumerable<ReadProductDto>>(products.Item1), products.Item2);
            
        return ResultBuilder.SuccessResult(productsReadDto);
    }
}