namespace Application.Common.Dtos.Category;

public record UpdateCategoryDto(
    Guid Id,
    string? Name,
    string? Description);