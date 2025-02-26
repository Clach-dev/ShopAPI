using Application.Common.Dtos.User;
using FluentValidation;

namespace Presentation.Common.Validators.User;

public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GuidRule();
        
        RuleFor(x => x.Role)
            .RoleRule();
    }
}