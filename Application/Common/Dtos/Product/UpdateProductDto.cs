namespace Application.Common.Dtos.Product;

/// <summary>
/// Dto for Product Update operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Product</param>
/// <param name="Name">string that contains Name of Product</param>
/// <param name="Description">string that contains Description of Product</param>
/// <param name="Price">decimal that contains Price of Product</param>
/// <param name="Amount">int that contains Amount of Product</param>
/// <param name="CategoryIds">IEnumerable_Guid that contains identifiers of Categories</param>
public record UpdateProductDto(
    Guid Id,
    string? Name,
    string? Description,
    decimal? Price,
    int? Amount,
    IEnumerable<Guid>? CategoryIds);