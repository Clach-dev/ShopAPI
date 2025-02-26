using Application.Common.Dtos.Product;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.ProductCases.Queries.GetProductsByFilterQuery;

public class GetProductsByFilterHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) 
    : IRequestHandler<GetProductsByFilterQuery, Result<ReadProductsDto>>
{
    public async Task<Result<ReadProductsDto>> Handle(
        GetProductsByFilterQuery getProductsByFilterQuery,
        CancellationToken cancellationToken)
    {
        var products = await unitOfWork.Products.GetByPredicateAsync(product =>
                (getProductsByFilterQuery.Name == null ||
                 product.Name.Contains(getProductsByFilterQuery.Name)) &&
                (!getProductsByFilterQuery.CategoryIds.Any() ||
                 (getProductsByFilterQuery.MaxPrice == null ||
                 product.Price <= getProductsByFilterQuery.MaxPrice) ||
                 (getProductsByFilterQuery.MinPrice == null ||
                  product.Price >= getProductsByFilterQuery.MinPrice) ||
                 getProductsByFilterQuery.CategoryIds.All(categoryId =>
                     product.Categories != null &&
                     product.Categories.Select(b => b.Id).Contains(categoryId))),
            mapper.Map<PageInfo>(getProductsByFilterQuery.PageInfoDto),
            cancellationToken);
        
        var productsReadDto = new ReadProductsDto(mapper.Map<IEnumerable<ReadProductDto>>(products.Item1), products.Item2);
        
        return ResultBuilder.SuccessResult(productsReadDto);
    }
}