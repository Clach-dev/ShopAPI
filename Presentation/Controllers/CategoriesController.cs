using Application.Common.Dtos;
using Application.Common.Dtos.Category;
using Application.UseCases.CategoryCases.Commands.CreateCategoryCase;
using Application.UseCases.CategoryCases.Commands.DeleteCategoryCase;
using Application.UseCases.CategoryCases.Commands.UpdateCategoryCase;
using Application.UseCases.CategoryCases.Queries.GetAllCategoriesCase;
using Application.UseCases.CategoryCases.Queries.GetCategoryByIdCase;
using AutoMapper;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CategoriesController(
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IMediator mediator)
    : CustomControllerBase(httpContextAccessor)
{
    /// <summary>
    /// Get all categories operation
    /// </summary>
    /// <param name="pageInfoDto">PageInfoDto which contains number of current page and number of items per page</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with categories information</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] PageInfoDto pageInfoDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllCategoriesQuery(pageInfoDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Get category by id operation
    /// </summary>
    /// <param name="categoryId">Guid identifier of category</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with category information</returns>
    [HttpGet("{categoryId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById(
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Creation of new category
    /// </summary>
    /// <param name="createCategoryDto">createCategoryDto which contains new category information</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with created category information</returns>
    [HttpPost]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryDto createCategoryDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<CreateCategoryCommand>(createCategoryDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Category update operation
    /// </summary>
    /// <param name="updateCategoryDto">updateCategoryDto which contains new information of existed category</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with updated category information</returns>
    [HttpPut]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] UpdateCategoryDto updateCategoryDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<UpdateCategoryCommand>(updateCategoryDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Delete operation of Category
    /// </summary>
    /// <param name="categoryId">Guid which contains id of category you want to delete</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with status code of delete operation</returns>
    [HttpDelete("{categoryId:guid}")]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> DeleteCategory(
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
        
        return Result(result);
    }
}