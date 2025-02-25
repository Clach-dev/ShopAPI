using Application.Common.Dtos;
using Application.Common.Dtos.Category;
using Application.Common.Utils;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetCategoriesByName;

public record GetCategoriesByNameQuery(
    string Name,
    PageInfoDto PageInfoDto)
    : IRequest<Result<IEnumerable<ReadCategoryDto>>>;