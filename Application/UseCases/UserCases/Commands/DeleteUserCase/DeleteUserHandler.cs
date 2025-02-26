using Application.Common.Utils;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.UserCases.Commands.DeleteUserCase;

public class DeleteUserHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, Result<byte?>>
{
    public async Task<Result<byte?>> Handle(    
        DeleteUserCommand deleteUserCommand,
        CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByIdAsync(deleteUserCommand.Id, cancellationToken);
        if (user is null)
        {
            return ResultBuilder.NotFoundResult<byte?>(ErrorMessages.UserIdNotFoundError);
        }
        
        await unitOfWork.Users.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultBuilder.NoContentResult<byte?>();
    }
}