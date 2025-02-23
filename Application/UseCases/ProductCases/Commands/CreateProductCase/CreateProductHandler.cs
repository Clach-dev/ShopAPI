using Application.Common.Dtos.Product;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.ProductCases.Commands.CreateProductCase;

public class CreateProductHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateProductCommand, Result<ReadProductDto>>
{
    public async Task<Result<ReadProductDto>> Handle(
        CreateProductCommand createProductCommand,
        CancellationToken cancellationToken)
    {
        var existingProduct = (await unitOfWork
                .Products
                .GetByPredicateAsync(product => product.Name == createProductCommand.Name &&
                                                product.Description == createProductCommand.Description &&
                                                product.Price.CompareTo(createProductCommand.Price) == 0 ,
                    new PageInfo(),
                    cancellationToken)).Item1
            .FirstOrDefault();
        if (existingProduct is not null)
        {
            return ResultBuilder.ConflictResult<ReadProductDto>(ErrorMessages.ExistingProductError);
        }

        var newProduct = mapper.Map<Product>(createProductCommand);

        await unitOfWork.Products.CreateAsync(newProduct, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var productReadDto = mapper.Map<ReadProductDto>(newProduct);
        return ResultBuilder.CreatedResult(productReadDto);
    }
}