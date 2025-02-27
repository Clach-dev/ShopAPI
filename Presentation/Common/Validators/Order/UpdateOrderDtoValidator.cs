using Application.Common.Dtos.Order;
using FluentValidation;

namespace Presentation.Common.Validators.Order;

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();
        
        RuleFor(x => x.DeliveryDate)!
            .NullableDeliveryDateRule();
         
        RuleFor(x => x.TotalPrice)!
            .NullableTotalPriceRule();
        
        RuleFor(x => x.Status)!
            .StatusRule();
        
        RuleFor(x => x.UserId)!
            .NullableGuidRule();
    }
}