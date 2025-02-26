using Application.Common.Dtos.Product;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.ProductCases.Queries.GetProductByIdQuery;

public record GetProductByIdQuery(
    Guid ProductId)
    : IRequest<Result<ReadProductDto>>;