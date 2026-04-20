using FinancialManagement.Blazor.Models.Reports;

namespace FinancialManagement.Blazor.Services.Reports;

public sealed class ReportApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReportApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<UnpaidInvoiceDto>> GetUnpaidInvoicesAsync(
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var result = await client.GetFromJsonAsync<List<UnpaidInvoiceDto>>(
            "api/reports/unpaid-invoices",
            cancellationToken);

        return result ?? [];
    }
}