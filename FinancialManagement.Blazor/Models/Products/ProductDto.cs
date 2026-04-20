namespace FinancialManagement.Blazor.Models.Products;

public sealed record ProductDto(
    int Id,
    string Code,
    string Name,
    decimal UnitPrice,
    bool IsActive);