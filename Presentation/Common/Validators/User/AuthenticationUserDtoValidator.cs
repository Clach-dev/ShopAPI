using Application.Common.Dtos.User;
using FluentValidation;

namespace Presentation.Common.Validators.User;

public class AuthenticationUserDtoValidator : AbstractValidator<AuthenticationUserDto>
{
    public AuthenticationUserDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmptyRule()
            .PhoneNumberRule();
        
        RuleFor(x => x.Password)
            .NotEmptyRule()
            .PasswordRule();
    }
}