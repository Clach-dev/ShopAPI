namespace Application.Common.Dtos.Product;

public record CreateProductDto(
    string Name,
    string? Description,
    bool Price,
    int Amount);