using Application.Common.Dtos.User;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.UserCases.Queries.GetUserByIdCase;

public record GetUserByIdQuery(
    Guid Id)
    : IRequest<Result<ReadUserDto>>;