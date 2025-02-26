using Application.Common.Dtos;
using Application.Common.Dtos.Product;
using Application.UseCases.ProductCases.Commands.CreateProductCase;
using Application.UseCases.ProductCases.Commands.DeleteProductCase;
using Application.UseCases.ProductCases.Commands.UpdateProductCase;
using Application.UseCases.ProductCases.Queries.GetAllProductsQuery;
using Application.UseCases.ProductCases.Queries.GetProductByIdQuery;
using Application.UseCases.ProductCases.Queries.GetProductsByFilterQuery;
using AutoMapper;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductsController(
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IMediator mediator)
    : CustomControllerBase(httpContextAccessor)
{
    /// <summary>
    /// Get all products operation
    /// </summary>
    /// <param name="pageInfoDto">PageInfoDto which contains number of current page and number of items per page</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with products information</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] PageInfoDto pageInfoDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllProductsQuery(pageInfoDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Get product by id operation
    /// </summary>
    /// <param name="productId">Guid identifier of product</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with products information</returns>
    [HttpGet("{productId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(productId), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Get filtered products operation
    /// </summary>
    /// <param name="getProductsByFilterDto">GetProductsByFilterDto which contains filter information</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with filtered products information</returns>
    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFilteredProducts(
        [FromQuery] GetProductsByFilterDto getProductsByFilterDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<GetProductsByFilterQuery>(getProductsByFilterDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Creation of new product
    /// </summary>
    /// <param name="createProductDto">createProductDto which contains new product information</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with created product information</returns>
    [HttpPost]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductDto createProductDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<CreateProductCommand>(createProductDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Product update operation
    /// </summary>
    /// <param name="updateProductDto">updateProductDto which contains new information of existed product</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with updated product information</returns>
    [HttpPut]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> UpdateProduct(
        [FromBody] UpdateProductDto updateProductDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<UpdateProductCommand>(updateProductDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Delete operation of Product
    /// </summary>
    /// <param name="productId">Guid which contains id of product you want to delete</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with status code of delete operation</returns>
    [HttpDelete("{productId:guid}")]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductCommand(productId), cancellationToken);
        
        return Result(result);
    }
}