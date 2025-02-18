using Application.Common.Dtos;
using FluentValidation;

namespace Presentation.Common.Validators;

public class PageInfoDtoValidator : AbstractValidator<PageInfoDto>
{
    public PageInfoDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .PageSettingsRule();

        RuleFor(x => x.PageSize)
            .PageSettingsRule();
    }
}