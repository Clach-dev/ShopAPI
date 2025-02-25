using Application.Common.Dtos.Order;
using Domain.Entities;
using FluentValidation;

namespace Presentation.Common.Validators.Order;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.TotalPrice)
            .NullableTotalPriceRule();
        
        RuleFor(x => x.Status)
            .StatusRule();
        
        RuleFor(x => x.DeliveryDate)
            .NullableDeliveryDateRule(); 
        
        RuleFor(x => x.UserId)
            .GuidRule();

        RuleFor(x => x.OrderItemIds)!
            .NullableGuidListRule();
    }
}