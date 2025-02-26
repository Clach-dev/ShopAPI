using Application.Common.Dtos.Order;
using FluentValidation;

namespace Presentation.Common.Validators.Order;

public class GetOrdersByFilterDtoValidator : AbstractValidator<GetOrderByFilterDto>
{
    public GetOrdersByFilterDtoValidator()
    {
        RuleFor(x => x.Status)!
            .StatusRule();
         
         RuleFor(x => x.DeliveryDate)!
             .NullableDeliveryDateRule();
    }
}