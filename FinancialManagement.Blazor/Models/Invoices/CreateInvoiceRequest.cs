namespace FinancialManagement.Blazor.Models.Invoices;

public sealed record CreateInvoiceRequest(
    int CustomerId,
    DateTime InvoiceDate,
    string? Notes,
    List<CreateInvoiceItemRequest> Items);

public sealed record CreateInvoiceItemRequest(
    int ProductId,
    decimal Quantity,
    decimal UnitPrice);
