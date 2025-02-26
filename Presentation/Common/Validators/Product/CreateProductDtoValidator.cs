using Application.Common.Dtos.Product;
using FluentValidation;

namespace Presentation.Common.Validators.Product;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyRule()
            .TitleRule();
        
        RuleFor(x => x.Description)!
            .DescriptionRule();
        
        RuleFor(x => x.Price)
            .PriceRule();

        RuleFor(x => x.Amount)
            .AmountRule();
    }
}
