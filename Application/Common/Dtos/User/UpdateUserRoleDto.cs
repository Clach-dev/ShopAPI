using Domain.Enums;

namespace Application.Common.Dtos.User;

/// <summary>
/// Dto for User Role Update operation
/// </summary>
/// <param name="UserId">Guid that contains identifier of User</param>
/// <param name="Role">Enum which represents Role of User</param>
public record UpdateUserRoleDto(
    Guid UserId,
    Roles Role);