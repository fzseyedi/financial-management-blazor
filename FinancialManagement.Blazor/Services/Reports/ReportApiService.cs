using FinancialManagement.Blazor.Models.Reports;
using FinancialManagement.Blazor.Services.Auth;

namespace FinancialManagement.Blazor.Services.Reports;

public sealed class ReportApiService
{
    private readonly FinancialApiClient _apiClient;

    public ReportApiService(FinancialApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<UnpaidInvoiceDto>> GetUnpaidInvoicesAsync(
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var result = await client.GetFromJsonAsync<List<UnpaidInvoiceDto>>(
            "api/reports/unpaid-invoices",
            cancellationToken);

        return result ?? [];
    }
}