namespace Application.Common.Dtos.Category;

public record CreateCategoryDto(
    string Name,
    string? Description);