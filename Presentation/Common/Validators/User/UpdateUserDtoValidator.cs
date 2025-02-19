using Application.Common.Dtos.User;
using FluentValidation;

namespace Presentation.Common.Validators.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();

        RuleFor(x => x.PhoneNumber)!
            .PhoneNumberRule();

        RuleFor(x => x.Password)!
            .PasswordRule();

        RuleFor(x => x.LastName)!
            .LastNameRule();

        RuleFor(x => x.FirstName)!
            .FirstNameRule();

        RuleFor(x => x.MiddleName)
            .MiddleNameRule();

        RuleFor(x => x.BirthDate)
            .DateOfBirthRule();

        RuleFor(x => x.Email)
            .EmailRule();
    }
}