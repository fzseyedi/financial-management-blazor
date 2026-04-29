using FinancialManagement.Blazor.Models.Common;
using FinancialManagement.Blazor.Models.Invoices;
using FinancialManagement.Blazor.Services.Auth;
using Microsoft.AspNetCore.WebUtilities;

namespace FinancialManagement.Blazor.Services.Invoices;

public sealed class InvoiceApiService
{
    private readonly FinancialApiClient _apiClient;

    public InvoiceApiService(FinancialApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PaginatedResponse<InvoiceDto>> GetAllPagedAsync(
        int? customerId = null,
        string? customerName = null,
        bool includeIssued = false,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var queryParams = new Dictionary<string, string?>
        {
            ["includeIssued"] = includeIssued.ToString().ToLowerInvariant(),
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = pageSize.ToString()
        };        

        if (customerId.HasValue)
            queryParams["customerId"] = customerId.Value.ToString();

        if (!string.IsNullOrWhiteSpace(customerName))
            queryParams["customerName"] = customerName;

        if (dateFrom.HasValue)
            queryParams["dateFrom"] = dateFrom.Value.ToString("yyyy-MM-dd");

        if (dateTo.HasValue)
            queryParams["dateTo"] = dateTo.Value.ToString("yyyy-MM-dd");

        var url = QueryHelpers.AddQueryString("api/invoices", queryParams);

        var result = await client.GetFromJsonAsync<PaginatedResponse<InvoiceDto>>(url, cancellationToken);
        return result ?? new PaginatedResponse<InvoiceDto>([], 0, pageNumber, pageSize, 0);
    }

    public async Task IssueAsync(
        int invoiceId,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.PostAsync($"api/invoices/{invoiceId}/issue", null, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Invoice issue failed."
                    : errorText);
        }
    }

    public async Task<int> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.PostAsJsonAsync("api/invoices", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Invoice creation failed."
                    : errorText);
        }

        var payload = await response.Content.ReadFromJsonAsync<IdResponse>(cancellationToken: cancellationToken);
        return payload?.Id ?? 0;
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.DeleteAsync($"api/invoices/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Invoice deletion failed."
                    : errorText);
        }
    }

    public async Task<InvoiceDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        return await client.GetFromJsonAsync<InvoiceDto>($"api/invoices/{id}", cancellationToken);
    }

    public async Task UpdateAsync(
        int id,
        UpdateInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.PutAsJsonAsync($"api/invoices/{id}", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Invoice update failed."
                    : errorText);
        }
    }

    private sealed record IdResponse(int Id);
}
