namespace Application.Common.Dtos.Category;

/// <summary>
/// Dto for Category Create operation
/// </summary>
/// <param name="Name">string which contains Name of Category</param>
/// <param name="Description">string which contains Description of Category</param>
public record CreateCategoryDto(
    string Name,
    string? Description);