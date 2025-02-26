namespace Application.Common.Dtos.Category;

/// <summary>
/// Dto for Category Update operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Category</param>
/// <param name="Name">string that contains Name of Category</param>
/// <param name="Description">string that contains Description of Category</param>
public record UpdateCategoryDto(
    Guid Id,
    string? Name,
    string? Description);