namespace Application.Common.Dtos.Product;

public record GetProductsByFilterDto(
    string? Name,
    double? MinPrice,
    double? MaxPrice,
    IEnumerable<Guid>? CategoryIds,
    PageInfoDto PageInfoDto);
