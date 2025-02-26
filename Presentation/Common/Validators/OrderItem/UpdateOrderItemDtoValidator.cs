using Application.Common.Dtos.OrderItem;
using FluentValidation;

namespace Presentation.Common.Validators.OrderItem;

public class UpdateOrderItemDtoValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)!
            .NullableGuidRule();
        
        RuleFor(x => x.OrderId)!
            .NullableGuidRule();
        
        RuleFor(x => x.Amount)!
            .NotNull().WithMessage("Amount is required.")
            .AmountRule();
    }
}