using Application.Common.Dtos.Order;
using FluentValidation;

namespace Presentation.Common.Validators.Order;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.TotalPrice)
            .NullableTotalPriceRule();
        
        RuleFor(x => x.Status)
            .NotEmptyRule()
            .StatusRule();
        
        RuleFor(x => x.DeliveryDate)
            .NullableDeliveryDateRule(); 
        
        RuleFor(x => x.UserId)
            .GuidRule();
    }
}