using Application.Common.Dtos;
using Application.Common.Dtos.OrderItem;
using Application.UseCases.OrderItemCases.Commands.CreateOrderItemCase;
using Application.UseCases.OrderItemCases.Commands.DeleteOrderItemCase;
using Application.UseCases.OrderItemCases.Commands.UpdateOrderItemCase;
using Application.UseCases.OrderItemCases.Queries.GetAllOrderItemsQuery;
using Application.UseCases.OrderItemCases.Queries.GetOrderItemByIdQuery;
using AutoMapper;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/[controller]")]
[ApiController]

public class OrderItemsController(
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IMediator mediator)
    : CustomControllerBase(httpContextAccessor)
{
    /// <summary>
    /// Get all orderItems operation
    /// </summary>
    /// <param name="pageInfoDto">PageInfoDto which contains number of current page and number of items per page</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with orderItems information</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllOrderItems(
        [FromQuery] PageInfoDto pageInfoDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllOrderItemsQuery(pageInfoDto), cancellationToken);
        return Result(result);
    }
    
    /// <summary>
    /// Get orderItem by id operation
    /// </summary>
    /// <param name="orderItemId">Guid identifier of orderItem</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with orderItems information</returns>
    [HttpGet("{orderItemId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrderItemById(
        [FromRoute] Guid orderItemId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderItemByIdQuery(orderItemId), cancellationToken);
        return Result(result);
    }
    
    /// <summary>
    /// Creation of new orderItem
    /// </summary>
    /// <param name="createOrderItemDto">createOrderItemDto which contains new orderItem information</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with created orderItem information</returns>
    [HttpPost]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> CreateOrderItem(
        [FromBody] CreateOrderItemDto createOrderItemDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<CreateOrderItemCommand>(createOrderItemDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// OrderItem update operation
    /// </summary>
    /// <param name="updateOrderItemDto">updateOrderItemDto which contains new information of existed orderItem</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with updated OrderItem information</returns>
    [HttpPut]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> UpdateOrderItem(
        [FromBody] UpdateOrderItemDto updateOrderItemDto,
        CancellationToken cancellationToken)
    {
        var updateOrderItemCommand = mapper.Map<UpdateOrderItemCommand>(updateOrderItemDto);
        
        var result = await mediator.Send(updateOrderItemCommand, cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Delete operation of OrderItem
    /// </summary>
    /// <param name="deleteOrderItemDto">deleteOrderItemDto which contains id of user you want to delete</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with status code of delete operation</returns>
    [HttpDelete]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> DeleteOrderItem(
        [FromBody] DeleteOrderItemDto deleteOrderItemDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<DeleteOrderItemCommand>(deleteOrderItemDto), cancellationToken);
        
        return Result(result);
    }
}