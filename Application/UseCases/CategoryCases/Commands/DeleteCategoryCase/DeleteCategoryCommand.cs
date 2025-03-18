using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.CategoryCases.Commands.DeleteCategoryCase;

public record DeleteCategoryCommand(
    Guid Id)
    : IRequest<Result<Unit>>;