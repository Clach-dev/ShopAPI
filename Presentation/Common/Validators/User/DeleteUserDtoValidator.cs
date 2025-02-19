using Application.Common.Dtos.User;
using FluentValidation;

namespace Presentation.Common.Validators.User;

public class DeleteUserDtoValidator : AbstractValidator<DeleteUserDto>
{
    public DeleteUserDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();
    }
}