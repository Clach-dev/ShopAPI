namespace Application.Common.Dtos.Category;

public record ReadCategoryDto(
    Guid Id,
    string Name,
    string? Description);