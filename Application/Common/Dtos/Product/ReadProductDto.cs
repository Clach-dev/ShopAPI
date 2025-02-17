namespace Application.Common.Dtos.Product;

public record ReadProductDto(
    Guid Id,
    string Name,
    string? Description,
    bool Price,
    int Amount);