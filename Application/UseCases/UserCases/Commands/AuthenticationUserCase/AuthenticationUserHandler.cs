using Application.Common.Dtos.Token;
using Application.Common.Utils;
using Domain.Entities;
using Domain.Interfaces.IAlgorithms;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.UserCases.Commands.AuthenticationUserCase;

public class AuthenticationUserHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokensGenerator tokensGenerator)
    : IRequestHandler<AuthenticationUserCommand, Result<ReadTokenDto>>
{
    public async Task<Result<ReadTokenDto>> Handle(
        AuthenticationUserCommand authenticationUserCommand,
        CancellationToken cancellationToken)
    {
        var user = (await unitOfWork
                .Users
                .GetByPredicateAsync(user => user.PhoneNumber == authenticationUserCommand.PhoneNumber, new PageInfo(), cancellationToken))
            .FirstOrDefault();
        if (user is null)
        {
            return ResultBuilder.NotFoundResult<ReadTokenDto>(ErrorMessages.UserPhoneNotFoundError);
        }

        if (!passwordHasher.VerifyHashedPassword(user.Password, authenticationUserCommand.Password))
        {
            return ResultBuilder.UnauthorizedResult<ReadTokenDto>(ErrorMessages.WrongPasswordError);
        }
        
        var accessToken = tokensGenerator.GenerateAccessToken(user);
        var refreshToken = tokensGenerator.GenerateRefreshToken(user);

        await unitOfWork.RefreshTokens.CreateAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var tokenReadDto = new ReadTokenDto(accessToken, refreshToken.Token.ToString());
        return ResultBuilder.CreatedResult(tokenReadDto);
    }
}   