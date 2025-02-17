namespace Application.Common.Dtos.Product;

public record UpdateProductDto(
    Guid Id,
    string? Name,
    string? Description,
    bool? Price,
    int? Amount);