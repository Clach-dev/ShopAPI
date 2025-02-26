using Application.Common.Dtos.User;
using Application.Common.Utils;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.UserCases.Commands.UpdateUserRoleCase;

public record UpdateUserRoleCommand(
    Guid UserId,
    Roles Role)
    : IRequest<Result<ReadUserRoleDto>>;