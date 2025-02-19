using Application.Common.Dtos.User;
using FluentValidation;

namespace Presentation.Common.Validators.User;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmptyRule()
            .PhoneNumberRule();

        RuleFor(x => x.Password)
            .NotEmptyRule()
            .PasswordRule();

        RuleFor(x => x.LastName)
            .NotEmptyRule()
            .LastNameRule();

        RuleFor(x => x.FirstName)
            .NotEmptyRule()
            .FirstNameRule();

        RuleFor(x => x.MiddleName)
            .MiddleNameRule();

        RuleFor(x => x.BirthDate)
            .DateOfBirthRule();

        RuleFor(x => x.Email)
            .EmailRule();
    }
}