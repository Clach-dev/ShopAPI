using Application.Common.Dtos.Product;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.ProductCases.Commands.UpdateProductCase;

public record UpdateProductCommand(
    Guid Id,
    string? Name,
    string? Description,
    decimal? Price,
    int? Amount,
    IEnumerable<Guid>? CategoryIds)
    : IRequest<Result<ReadProductDto>>;