namespace FinancialManagement.Blazor.Models.Reports;

public sealed record UnpaidInvoiceDto(
    int InvoiceId,
    string InvoiceNumber,
    int CustomerId,
    string CustomerName,
    DateTime InvoiceDate,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    int Status);