using Application.Common.Dtos.Category;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.CategoryCases.Queries.GetCategoriesByIdCase;

public record GetCategoryByIdQuery(
    Guid Id)
    : IRequest<Result<ReadCategoryDto>>;