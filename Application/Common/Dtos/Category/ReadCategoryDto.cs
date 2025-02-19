namespace Application.Common.Dtos.Category;

/// <summary>
/// Dto for Category Read operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Category</param>
/// <param name="Name">string that contains Name of Category</param>
/// <param name="Description">string that contains Description of Categoryx</param>
public record ReadCategoryDto(
    Guid Id,
    string Name,
    string? Description);