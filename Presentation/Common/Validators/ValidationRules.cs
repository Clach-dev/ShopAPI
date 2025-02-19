using Domain.Enums;
using FluentValidation;

namespace Presentation.Common.Validators;

public static class ValidationRules
{
    private const string NameRegex = "^[a-zA-Z'-]+$";
    
    public static IRuleBuilder<T, TProperty> NotEmptyRule<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        where TProperty : class 
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"The {typeof(TProperty).Name} is required.");
    }
    
    public static IRuleBuilder<T, int> PageSettingsRule<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage("Page settings cannot be null.")
            .Must(x => x > 0).WithMessage("Page number must be greater than 0.");
    }
    
    public static IRuleBuilder<T, Guid> GuidRule<T>(this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(Guid.Empty);
    }
    
    public static IRuleBuilder<T, string> LastNameRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(50).WithMessage("The last name must not exceed 20 characters.")
            .Matches(NameRegex).WithMessage("The last name can only contain letters, apostrophes, and hyphens.");
    }
    
    public static IRuleBuilder<T, string> FirstNameRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(50).WithMessage("The first name must not exceed 20 characters.")
            .Matches(NameRegex).WithMessage("The first name can only contain letters, apostrophes, and hyphens.");
    }
    
    public static IRuleBuilder<T, string?> MiddleNameRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(50).WithMessage("The middle name must not exceed 20 characters.")
            .Matches(NameRegex).WithMessage("The middle name can only contain letters, apostrophes, and hyphens.");
    }
    
    public static IRuleBuilder<T, DateTime?> DateOfBirthRule<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
    {
        return ruleBuilder
            .LessThan(DateTime.Now).WithMessage("The date of birth must be in the past.");
    }

    public static IRuleBuilder<T, string> PhoneNumberRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("The PhoneNumber of birth is required.")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Phone number must have from 10 to 15 numbers and can starts with '+'.");
    }
    
    public static IRuleBuilder<T, string> PasswordRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("The Password of birth is required.")
            .MinimumLength(8).WithMessage("Password must have at least 8 symbols.")
            .Matches("[A-Z]").WithMessage("Password must have at least one upper case latter.")
            .Matches("[a-z]").WithMessage("Password must have at least one lower case latter.")
            .Matches("[0-9]").WithMessage("Password must have at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must have at least one special symbol.");
    }
    
    public static IRuleBuilder<T, string?> EmailRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .EmailAddress().WithMessage("Wrong email format.");
    }
    
    public static IRuleBuilder<T, Roles> RoleRule<T>(this IRuleBuilder<T, Roles> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("The role is required")
            .Must(role => Enum.IsDefined(typeof(Roles), role))
            .WithMessage("Role must be a valid value.");
    }
}