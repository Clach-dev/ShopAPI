using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Application.Common.Dtos.Product;

/// <summary>
/// Dto for Product Create operation
/// </summary>
/// <param name="Name">string that contains Name of Product</param>
/// <param name="Description">string that contains Description of Product</param>
/// <param name="Price">decimal that contains Price of Product</param>
/// <param name="Amount">int that contains Amount of Product</param>
/// <param name="CategoryIds">IEnumerable_Guid that contains identifiers of Categories</param>
/// <param name="Image">IFormFile that contains Product Image</param>
public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int Amount,
    IEnumerable<Guid>? CategoryIds,
    IFormFile? Image);