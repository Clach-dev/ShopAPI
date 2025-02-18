namespace Application.Common.Dtos.Product;

/// <summary>
/// Dto for Product Delete operation 
/// </summary>
/// <param name="Id">Guid that contains identifier of Product</param>
public record DeleteProductDto(
    Guid Id);