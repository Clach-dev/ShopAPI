namespace Application.Common.Dtos.Category;

/// <summary>
/// Dto for Categories Read operation
/// </summary>
/// <param name="Categories">IEnumerable_ReadCategoryDto that contains list of ReadCategoryDto</param>
/// <param name="TotalCount">int that contains total count of categories</param>
public record ReadCategoriesDto(
    IEnumerable<ReadCategoryDto> Categories,
    int TotalCount);