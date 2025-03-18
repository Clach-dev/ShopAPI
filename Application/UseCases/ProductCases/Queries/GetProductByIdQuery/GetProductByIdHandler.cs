using Application.Common.Dtos.Product;
using Application.Common.Utils;
using AutoMapper;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.ProductCases.Queries.GetProductByIdQuery;

public class GetProductByIdHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) 
    : IRequestHandler<GetProductByIdQuery, Result<ReadProductDto>>
{
    public async Task<Result<ReadProductDto>> Handle(
        GetProductByIdQuery getProductByIdQuery,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(getProductByIdQuery.ProductId, cancellationToken);
        if (product is null)
        {
            return ResultBuilder.NotFoundResult<ReadProductDto>(ErrorMessages.ProductIdNotFoundError);
        }
        
        if (product.ImageUri != null)
        {
            product.ImageUri = unitOfWork.ProductImages.GetReadOnlyImageUri(product.ImageUri);
        }
        
        var productReadDto = mapper.Map<ReadProductDto>(product);
        return ResultBuilder.SuccessResult(productReadDto);
    }
}