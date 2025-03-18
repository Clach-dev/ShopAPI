using System.Transactions;
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
    if (existingProduct is not null && existingProduct.Id != updateProductCommand.Id)
    {
        return ResultBuilder.ConflictResult<ReadProductDto>(ErrorMessages.ExistingProductError);
    }

    if (updateProductCommand.CategoryIds != null)
    {
        var existingCategories = await unitOfWork.Categories.GetByPredicateAsync(category =>
                updateProductCommand.CategoryIds.Contains(category.Id),
            new PageInfo { PageNumber = 1, PageSize = updateProductCommand.CategoryIds.Count() },
            cancellationToken);
        
        if (existingCategories.Item1.Count() != updateProductCommand.CategoryIds.Count())
        {
            return ResultBuilder.ConflictResult<ReadProductDto>(ErrorMessages.CategoryConflictError);
        }
        currentProduct.Categories = existingCategories.Item1;
    }

    using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    try
    {
        Uri? oldImageUri = currentProduct.ImageUri;
        Uri? newImageUri = null;

        if (updateProductCommand.Image is not null)
        {
            await using var imageStream = updateProductCommand.Image.OpenReadStream();
                
            newImageUri = await unitOfWork.ProductImages.UploadFileAsync(imageStream, cancellationToken);

            currentProduct.ImageUri = newImageUri;
        }

        mapper.Map(updateProductCommand, currentProduct);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (oldImageUri is not null && newImageUri is not null)
        {
            await unitOfWork.ProductImages.DeleteFileAsync(oldImageUri, cancellationToken);
        }
        
        transaction.Complete();

        if (currentProduct.ImageUri is not null)
        {
            currentProduct.ImageUri = unitOfWork.ProductImages.GetReadOnlyImageUri(currentProduct.ImageUri);
        }
        var productReadDto = mapper.Map<ReadProductDto>(currentProduct);
        
        return ResultBuilder.SuccessResult(productReadDto);
    }
    catch (Exception)
    {
        return ResultBuilder.InternalServerErrorResult<ReadProductDto>(ErrorMessages.ProductUpdateFailureError);
    }
}
}