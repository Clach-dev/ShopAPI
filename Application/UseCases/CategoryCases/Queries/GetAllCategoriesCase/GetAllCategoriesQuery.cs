using Application.Common.Dtos;
using Application.Common.Dtos.Category;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetAllCategoriesCase;

public record GetAllCategoriesQuery(
    PageInfoDto PageInfoDto)
    : IRequest<Result<ReadCategoriesDto>>;