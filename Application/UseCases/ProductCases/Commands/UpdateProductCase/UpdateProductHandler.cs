using Application.Common.Dtos.Product;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.ProductCases.Commands.UpdateProductCase;

public class UpdateProductHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) 
    : IRequestHandler<UpdateProductCommand, Result<ReadProductDto>>
{
    public async Task<Result<ReadProductDto>> Handle(
        UpdateProductCommand updateProductCommand,
        CancellationToken cancellationToken)
    {
        var currentProduct = await unitOfWork.Products.GetByIdAsync(updateProductCommand.Id, cancellationToken);
        if (currentProduct is null)
        {
            return ResultBuilder.NotFoundResult<ReadProductDto>(ErrorMessages.ProductIdNotFoundError);
        }
    
        var existingProduct = (await unitOfWork
                .Products
                .GetByPredicateAsync(product => product.Name == updateProductCommand.Name &&
                                                product.Description == updateProductCommand.Description &&
                                                product.Price == updateProductCommand.Price,
                    new PageInfo(),
                    cancellationToken)).Item1
            .FirstOrDefault();
        if (existingProduct is not null)
        {
            return ResultBuilder.ConflictResult<ReadProductDto>(ErrorMessages.ExistingProductError);
        }
    
        mapper.Map(updateProductCommand, currentProduct);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    
        var productReadDto = mapper.Map<ReadProductDto>(currentProduct);
        return ResultBuilder.SuccessResult(productReadDto);
    }
}