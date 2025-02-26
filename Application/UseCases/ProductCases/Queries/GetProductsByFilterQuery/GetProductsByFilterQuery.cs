using Application.Common.Dtos;
using Application.Common.Dtos.Product;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.ProductCases.Queries.GetProductsByFilterQuery;

public record GetProductsByFilterQuery(
string? Name,
decimal? MinPrice,
decimal? MaxPrice,
IEnumerable<Guid> CategoryIds,
PageInfoDto PageInfoDto) 
: IRequest<Result<ReadProductsDto>>;