using Application.Common.Dtos.OrderItem;
using FluentValidation;

namespace Presentation.Common.Validators.OrderItem;

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GuidRule();
        
        RuleFor(x => x.OrderId)
            .GuidRule();
        
        RuleFor(x => x.Amount)
            .AmountRule();
    }
}