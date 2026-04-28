namespace FinancialManagement.Blazor.Models.Invoices;

public sealed record InvoiceItemDto(
    int Id,
    int ProductId,
    string ProductCode,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal);
