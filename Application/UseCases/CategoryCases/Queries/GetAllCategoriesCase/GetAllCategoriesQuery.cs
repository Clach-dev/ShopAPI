using Application.Common.Dtos;
using Application.Common.Dtos.Category;
using Application.Common.Utils;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetAllCategoriesCase;

public record GetAllCategoriesQuery(
    PageInfoDto PageInfoDto)
    : PageInfoDto, IRequest<Result<IEnumerable<ReadCategoryDto>>>;