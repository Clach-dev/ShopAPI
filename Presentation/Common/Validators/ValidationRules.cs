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
    
    public static IRuleBuilder<T, Guid?> NullableGuidRule<T>(this IRuleBuilder<T, Guid?> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(Guid.Empty);
    }
    
    public static IRuleBuilder<T, string?> PhoneNumberRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
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

    public static IRuleBuilder<T, string?> EmailRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .EmailAddress().WithMessage("Wrong email format.");
    }
    
    public static IRuleBuilder<T, Roles> RoleRule<T>(this IRuleBuilder<T, Roles> ruleBuilder)
    {
        return ruleBuilder
            .Must(role => Enum.IsDefined(typeof(Roles), role))
            .WithMessage("Role must be a valid value.");
    }
    
    public static IRuleBuilder<T, string> TitleRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.
            MaximumLength(50).WithMessage("The description must not exceed 300 characters.");
    }
    
    public static IRuleBuilder<T, string> DescriptionRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.
            MaximumLength(300).WithMessage("The description must not exceed 300 characters.");
    }
    
    public static IRuleBuilder<T, decimal> PriceRule<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0m).WithMessage("Price must be greater than zero.")
            .LessThanOrEqualTo(1_000_000m).WithMessage("Price must not exceed 1,000,000.");
    }

    public static IRuleBuilder<T, decimal?> NullablePriceRule<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0m).WithMessage("Price must be greater than zero.")
            .LessThanOrEqualTo(1_000_000m).WithMessage("Price must not exceed 1,000,000.");
    }

    public static IRuleBuilder<T, int> AmountRule<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Amount is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative.");
    }

    public static IRuleBuilder<T, int?> NullableAmountRule<T>(this IRuleBuilder<T, int?> ruleBuilder)
    {
        return ruleBuilder
            .SetValidator(new InlineValidator<int?>()
            {
                v => v.RuleFor(x => x!.Value)
                    .GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative.")
            });
    }

    public static IRuleBuilder<T, string> StatusRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Matches("^[A-Z][a-z]{1,19}$").WithMessage("Status must start with an uppercase letter, " +
                                                       "contain only English letters, and be between " +
                                                       "2 and 20 characters long.");
    } 
    
    public static IRuleBuilder<T, decimal?> NullableTotalPriceRule<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("Total price must be greater than 0.")
            .LessThanOrEqualTo(1_000_000).WithMessage("Total price must not exceed 1,000,000.");
    } 
    
    public static IRuleBuilder<T, DateTime?> NullableDeliveryDateRule<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Delivery date must be in future")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMonths(3))
            .WithMessage("Delivery date cannot be more than 2 month from today.");
    }
}