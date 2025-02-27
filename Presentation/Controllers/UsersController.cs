using Application.Common.Dtos;
using Application.Common.Dtos.User;
using Application.UseCases.UserCases.Commands.AuthenticationUserCase;
using Application.UseCases.UserCases.Commands.DeleteUserCase;
using Application.UseCases.UserCases.Commands.RegisterUserCase;
using Application.UseCases.UserCases.Commands.UpdateUserCase;
using Application.UseCases.UserCases.Commands.UpdateUserRoleCase;
using Application.UseCases.UserCases.Queries.GetAllUsersCase;
using Application.UseCases.UserCases.Queries.GetUserByIdCase;
using AutoMapper;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UsersController(
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IMediator mediator)
    : CustomControllerBase(httpContextAccessor)
{
    /// <summary>
    /// Get all users operation
    /// </summary>
    /// <param name="pageInfoDto">PageInfoDto which contains number of current page and number of items per page</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with users information</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] PageInfoDto pageInfoDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllUsersQuery(pageInfoDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Get user by id operation
    /// </summary>
    /// <param name="userId">Guid identifier of user</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with user information</returns>
    [HttpGet("{userId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserById(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Registration of new user
    /// </summary>
    /// <param name="registerUserDto">RegisterUserDto which contains new user information</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with created user information</returns>
    [HttpPost("registration")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserDto registerUserDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<RegisterUserCommand>(registerUserDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Authentication of user
    /// </summary>
    /// <param name="authenticationUserDto">AuthenticationUserDto which contains user information for authentication</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with user tokens information</returns>
    [HttpPost("authentication")]
    [AllowAnonymous]
    public async Task<IActionResult> AuthenticateUser(
        [FromBody] AuthenticationUserDto authenticationUserDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<AuthenticationUserCommand>(authenticationUserDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// User update operation
    /// </summary>
    /// <param name="updateUserDto">UpdateUserDto which contains new information of existed user</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with updated user information</returns>
    [HttpPut]
    [Authorize(Policy = Policies.OnlyUserAccess)]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UpdateUserDto updateUserDto,
        CancellationToken cancellationToken)
    {
        var updateUserCommand = mapper.Map<UpdateUserCommand>(updateUserDto);
        updateUserCommand.Id = GetUserId();
        
        var result = await mediator.Send(updateUserCommand, cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Update operation of user role
    /// </summary>
    /// <param name="updateUserRoleDto">UpdateUserRoleDto</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with updated user role information</returns>
    [HttpPut("roles")]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> UpdateUserRole(
        [FromBody] UpdateUserRoleDto updateUserRoleDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<UpdateUserRoleCommand>(updateUserRoleDto), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Delete operation of user
    /// </summary>
    /// <param name="userId">Guid which contains id of user you want to delete</param>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with status code of delete operation</returns>
    [HttpDelete("{userId:guid}")]
    [Authorize(Policy = Policies.OnlyAdminAccess)]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUserCommand(userId), cancellationToken);
        
        return Result(result);
    }
    
    /// <summary>
    /// Delete operation of user himself
    /// </summary>
    /// <param name="cancellationToken">CancellationToken token of operation cancel</param>
    /// <returns>Result with status code of delete operation</returns>
    [HttpDelete("myself")]
    [Authorize(Policy = Policies.OnlyUserAccess)]
    public async Task<IActionResult> DeleteUser(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUserCommand(GetUserId()), cancellationToken);
        
        return Result(result);
    }
}