namespace Application.Common.Dtos.Product;

/// <summary>
/// Dto for Product Read operation
/// </summary>
/// <param name="Id">Guid that contains identifier of Product</param>
/// <param name="Name">string that contains Name of Product</param>
/// <param name="Description">string that contains Description of Product</param>
/// <param name="Price">double that contains Price of Product</param>
/// <param name="Amount">int that contains Amount of Product</param>
public record ReadProductDto(
    Guid Id,
    string Name,
    string? Description,
    double Price,
    int Amount);