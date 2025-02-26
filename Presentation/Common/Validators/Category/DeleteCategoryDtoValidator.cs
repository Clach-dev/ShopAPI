using Application.Common.Dtos.Category;
using FluentValidation;

namespace Presentation.Common.Validators.Category;

public class DeleteCategoryDtoValidator : AbstractValidator<DeleteCategoryDto>
{
    public DeleteCategoryDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();
    }
}