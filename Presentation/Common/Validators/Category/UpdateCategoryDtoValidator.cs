using Application.Common.Dtos.Category;
using FluentValidation;

namespace Presentation.Common.Validators.Category;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();
     
        RuleFor(x => x.Name)!
            .TitleRule();
        
        RuleFor(x => x.Description)!
            .DescriptionRule();
    }
}