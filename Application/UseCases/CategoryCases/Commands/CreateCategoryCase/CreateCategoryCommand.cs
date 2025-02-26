using Application.Common.Dtos.Category;
using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.CategoryCases.Commands.CreateCategoryCase;

public record CreateCategoryCommand(
    string Name,
    string? Description)
    : IRequest<Result<ReadCategoryDto>>;
