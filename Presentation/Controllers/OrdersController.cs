using Application.Common.Dtos;
using Application.Common.Dtos.Order;
using Application.UseCases.OrderCases.Commands.CreateOrderCase;
using Application.UseCases.OrderCases.Commands.DeleteOrderCase;
using Application.UseCases.OrderCases.Commands.UpdateOrderCase;
using Application.UseCases.OrderCases.Queries.GetAllOrdersCase;
using Application.UseCases.OrderCases.Queries.GetOrderByIdCase;
using Application.UseCases.OrderCases.Queries.GetOrdersByFilterCase;
using AutoMapper;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController(
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IMediator mediator) 
    : CustomControllerBase(httpContextAccessor)
{
    /// <summary>
    /// Get all orders operation
    /// </summary>
    /// <param name="pageInfoDto">PageInfoDto which contains number of current page and number of items per page</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with orders information</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] PageInfoDto pageInfoDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllOrdersQuery(pageInfoDto), cancellationToken);
        
        return Result(result);
    }

    /// <summary>
    /// Get order by id operation
    /// </summary>
    /// <param name="orderId">Guid identifier of order</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with order information</returns>
    [HttpGet("{orderId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrderById(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<GetOrderByIdQuery>(orderId), cancellationToken);
        
        return Result(result);
    }

    
    /// <summary>
    /// Get orders by filter operation
    /// </summary>
    /// <param name="getOrderByFilterDto">Dto containing order filter parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with filtered orders information</returns>
    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrdersByFilter(
        [FromQuery] GetOrderByFilterDto getOrderByFilterDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<GetOrdersByFilterQuery>(getOrderByFilterDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Create a new order operation
    /// </summary>
    /// <param name="createOrderDto">CreateOrderDto which contains new order information</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with created order information</returns>
    [HttpPost]
    [Authorize(Policy = Policies.AuthenticateAccess)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto createOrderDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<CreateOrderCommand>(createOrderDto), cancellationToken);
        return Result(result);
    }

    /// <summary>
    /// Update an existing order operation
    /// </summary>
    /// <param name="updateOrderDto">UpdateOrderDto which contains updated order information</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with updated order information</returns>
    [HttpPut]
    [Authorize(Policy = Policies.AuthenticateAccess)]
    public async Task<IActionResult> UpdateOrder(
        [FromBody] UpdateOrderDto updateOrderDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<UpdateOrderCommand>(updateOrderDto), cancellationToken);
        
        return Result(result);
    }

    /// <summary>
    /// Delete an order operation
    /// </summary>
    /// <param name="orderId">Guid which contains id of order to delete</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with status of delete operation</returns>
    [HttpDelete("{orderId:guid}")]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> DeleteOrder(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteOrderCommand(orderId), cancellationToken);
        return Result(result);
    }
}
