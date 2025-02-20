using Application.Common.Dtos.Order;
using FluentValidation;

namespace Presentation.Common.Validators.Order;

public class DeleteOrderDtoValidator  : AbstractValidator<DeleteOrderDto>
{
    public DeleteOrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();
    }
}