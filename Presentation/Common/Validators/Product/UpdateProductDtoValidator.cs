using Application.Common.Dtos.Product;
using FluentValidation;

namespace Presentation.Common.Validators.Product;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();
        
        RuleFor(x => x.Name)!
            .TitleRule();

        RuleFor(x => x.Description)!
            .DescriptionRule();

        RuleFor(x => x.Price)
            .NullablePriceRule();

        RuleFor(x => x.Amount)
            .NullableAmountRule();

        RuleFor(x => x.Image)
            .ValidImageFileRule();
    }
}
