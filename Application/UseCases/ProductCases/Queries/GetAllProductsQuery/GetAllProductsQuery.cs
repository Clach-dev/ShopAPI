using Application.Common.Dtos;
using Application.Common.Dtos.Product;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.ProductCases.Queries.GetAllProductsQuery;

public record GetAllProductsQuery(
    PageInfoDto PageInfoDto)
    : IRequest<Result<ReadProductsDto>>;