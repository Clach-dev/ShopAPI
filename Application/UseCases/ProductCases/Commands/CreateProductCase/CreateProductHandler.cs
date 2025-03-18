using System.Transactions;
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
                .GetByPredicateAsync(product => 
                        product.Name == createProductCommand.Name && 
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
        
        
        
        if (createProductCommand.CategoryIds != null)
        {
            var existingCategories = await unitOfWork.Categories.GetByPredicateAsync(category =>
                    createProductCommand.CategoryIds != null &&
                    createProductCommand.CategoryIds.Contains(category.Id),
                new PageInfo { PageNumber = 1, PageSize = createProductCommand.CategoryIds?.Count() ?? 0 },
                cancellationToken);
            
            if (existingCategories.Item1.Count() != createProductCommand.CategoryIds?.Count() ||
                existingCategories.Item1.Distinct().Count() != createProductCommand.CategoryIds?.Count())
            {
                return ResultBuilder.ConflictResult<ReadProductDto>(ErrorMessages.CategoryConflictError);
            }
            newProduct.Categories = existingCategories.Item1;
        }

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            await unitOfWork.Products.CreateAsync(newProduct, cancellationToken);
    
            if (createProductCommand.Image is not null)
            {
                await using var imageStream = createProductCommand.Image.OpenReadStream();
                
                var imageUri = await unitOfWork.ProductImages.UploadFileAsync(imageStream, cancellationToken);

                newProduct.ImageUri = imageUri;
            }
    
            await unitOfWork.SaveChangesAsync(cancellationToken);
            transaction.Complete();

            if (newProduct.ImageUri is not null)
            {
                newProduct.ImageUri = unitOfWork.ProductImages.GetReadOnlyImageUri(newProduct.ImageUri);
            }
            var productReadDto = mapper.Map<ReadProductDto>(newProduct);
            
            return ResultBuilder.CreatedResult(productReadDto);
        }
        catch (Exception)
        {
            return ResultBuilder.InternalServerErrorResult<ReadProductDto>(ErrorMessages.ProductCreationFailureError);
        }
    }
}