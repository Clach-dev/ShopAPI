using Application.Common.Dtos.Product;
using Application.UseCases.ProductCases.Commands.DeleteProductCase;
using FluentValidation;

namespace Presentation.Common.Validators.Product;

public class DeleteProductDtoValidator : AbstractValidator<DeleteProductDto>
{
    public DeleteProductDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GuidRule();
    }
}