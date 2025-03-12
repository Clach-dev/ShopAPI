using Application.Common.Dtos.Product;
using Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.ProductCases.Commands.UpdateProductCase;

public record UpdateProductCommand(
    Guid Id,
    string? Name,
    string? Description,
    decimal? Price,
    int? Amount,
    IEnumerable<Guid>? CategoryIds,
    IFormFile? Image)
    : IRequest<Result<ReadProductDto>>;