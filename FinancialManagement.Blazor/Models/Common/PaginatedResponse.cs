namespace FinancialManagement.Blazor.Models.Common;

public sealed record PaginatedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);
