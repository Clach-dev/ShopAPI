using Application.Common.Dtos.Category;
using FluentValidation;

namespace Presentation.Common.Validators.Category;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .TitleRule();
        
        RuleFor(x => x.Description)!
            .DescriptionRule();
    }
}