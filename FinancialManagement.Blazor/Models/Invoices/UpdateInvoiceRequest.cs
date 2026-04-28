namespace FinancialManagement.Blazor.Models.Invoices;

public sealed record UpdateInvoiceRequest(
    int CustomerId,
    DateTime InvoiceDate,
    string? Notes,
    string Version,
    string? ModifiedBy,
    List<UpdateInvoiceItemRequest> Items);

public sealed record UpdateInvoiceItemRequest(
    int Id,
    int ProductId,
    decimal Quantity,
    decimal UnitPrice);
