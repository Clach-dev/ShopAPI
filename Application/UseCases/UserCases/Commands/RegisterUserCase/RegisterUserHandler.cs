using Application.Common.Dtos.User;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IAlgorithms;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.UserCases.Commands.RegisterUserCase;

public class RegisterUserHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IMapper mapper)
    : IRequestHandler<RegisterUserCommand, Result<ReadUserDto>>
{
    public async Task<Result<ReadUserDto>> Handle(
        RegisterUserCommand registerUserCommand,
        CancellationToken cancellationToken)
    {
        var existingUser = (await unitOfWork
                .Users
                .GetByPredicateAsync(user => user.PhoneNumber == registerUserCommand.PhoneNumber, new PageInfo() , cancellationToken))
            .FirstOrDefault();
        if (existingUser is not null)
        {
            return ResultBuilder.ConflictResult<ReadUserDto>(ErrorMessages.ExistingUserPhoneNumberError);
        }

        var newUser = mapper.Map<User>(registerUserCommand);
        
        newUser.Password = passwordHasher.HashPassword(registerUserCommand.Password);
        
        await unitOfWork.Users.CreateAsync(newUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var userReadDto = mapper.Map<ReadUserDto>(newUser);
        return ResultBuilder.CreatedResult(userReadDto);
    }
}