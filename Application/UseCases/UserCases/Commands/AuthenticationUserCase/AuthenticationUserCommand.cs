using Application.Common.Dtos.Token;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.UserCases.Commands.AuthenticationUserCase;

public record AuthenticationUserCommand(
    string PhoneNumber,
    string Password)
    : IRequest<Result<ReadTokenDto>>;