using Application.Common.Dtos.User;
using Application.Common.Utils;
using AutoMapper;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.UserCases.Commands.UpdateUserRoleCase;

public class UpdateUserRoleHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<UpdateUserRoleCommand, Result<ReadUserRoleDto>>
{
    public async Task<Result<ReadUserRoleDto>> Handle(
        UpdateUserRoleCommand updateUserRoleCommand,
        CancellationToken cancellationToken)
    {   
        var user = await unitOfWork.Users.GetByIdAsync(updateUserRoleCommand.UserId, cancellationToken);
        if (user is null)
        {
            return ResultBuilder.NotFoundResult<ReadUserRoleDto>(ErrorMessages.UserIdNotFoundError);
        }

        user.Role = updateUserRoleCommand.Role;
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var userRoleReadDto = mapper.Map<ReadUserRoleDto>(user);
        return ResultBuilder.SuccessResult(userRoleReadDto);
    }
}