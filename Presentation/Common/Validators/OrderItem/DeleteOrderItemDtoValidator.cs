using Application.Common.Dtos.OrderItem;
using FluentValidation;

namespace Presentation.Common.Validators.OrderItem;

public class DeleteOrderItemDtoValidator : AbstractValidator<DeleteOrderItemDto>
{
    public DeleteOrderItemDtoValidator()
    {
        RuleFor(x => x.Id)
            .GuidRule();
    }
}