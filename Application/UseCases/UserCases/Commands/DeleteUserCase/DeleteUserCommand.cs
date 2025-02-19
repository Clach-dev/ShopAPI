using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.UserCases.Commands.DeleteUserCase;

public record DeleteUserCommand(
    Guid Id)
    : IRequest<Result<byte?>>;