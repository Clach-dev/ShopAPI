using Application.Common.Dtos.User;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.UserCases.Commands.UpdateUserCase;

public class UpdateUserHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<UpdateUserCommand, Result<ReadUserDto>>
{
    public async Task<Result<ReadUserDto>> Handle(
        UpdateUserCommand updateUserCommand,
        CancellationToken cancellationToken)
    {
        var currentUser = await unitOfWork.Users.GetByIdAsync(updateUserCommand.Id, cancellationToken);
        if (currentUser is null)
        {
            return ResultBuilder.NotFoundResult<ReadUserDto>(ErrorMessages.UserIdNotFoundError);
        }
        
        var existedUser = (await unitOfWork
                .Users
                .GetByPredicateAsync(user => user.PhoneNumber == updateUserCommand.PhoneNumber,
                    new PageInfo(),
                    cancellationToken)).Item1
            .FirstOrDefault();
        if (existedUser is not null && existedUser.Id != currentUser.Id)
        {
            return ResultBuilder.ConflictResult<ReadUserDto>(ErrorMessages.ExistingUserPhoneNumberError);
        }
        
        mapper.Map(updateUserCommand, currentUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var userReadDto = mapper.Map<ReadUserDto>(currentUser);
        return ResultBuilder.SuccessResult(userReadDto);
    }
}