using Application.Common.Dtos;
using FluentValidation;

namespace Presentation.Common.Validators;

public static class ValidationRules
{
    public static IRuleBuilder<T, int> PageSettingsRule<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage("Page settings cannot be null.")
            .Must(x => x > 0).WithMessage("Page number must be greater than 0.");
    }
}