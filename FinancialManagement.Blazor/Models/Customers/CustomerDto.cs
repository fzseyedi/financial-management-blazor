namespace FinancialManagement.Blazor.Models.Customers;

public sealed record CustomerDto(
    int Id,
    string Code,
    string Name,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive);
