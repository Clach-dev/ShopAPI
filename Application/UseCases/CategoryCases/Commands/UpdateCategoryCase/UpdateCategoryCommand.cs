using Application.Common.Dtos.Category;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.CategoryCases.Commands.UpdateCategoryCase;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description)
    : IRequest<Result<ReadCategoryDto>>;
