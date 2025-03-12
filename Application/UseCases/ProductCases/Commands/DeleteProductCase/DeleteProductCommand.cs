using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.ProductCases.Commands.DeleteProductCase;

public record DeleteProductCommand(
    Guid ProductId)
    : IRequest<Result<Unit>>;