namespace Application.Common.Dtos.Category;

/// <summary>
/// Dto for Category Delete operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Category</param>
public record DeleteCategoryDto(
    Guid Id);