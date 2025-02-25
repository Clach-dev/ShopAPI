using Application.Common.Dtos.Product;
using FluentValidation;

namespace Presentation.Common.Validators.Product;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .TitleRule();
        
        RuleFor(x => x.Description)!
            .DescriptionRule();
        
        RuleFor(x => x.Price)
            .NotNull().WithMessage("Price is required.")
            .PriceRule();
            

        RuleFor(x => x.Amount)
            .NotNull().WithMessage("Amount is required.")
            .AmountRule();
    }
}
