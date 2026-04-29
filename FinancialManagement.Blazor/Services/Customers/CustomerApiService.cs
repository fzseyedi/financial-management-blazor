using FinancialManagement.Blazor.Models.Common;
using FinancialManagement.Blazor.Models.Customers;
using FinancialManagement.Blazor.Services.Auth;
using Microsoft.AspNetCore.WebUtilities;

namespace FinancialManagement.Blazor.Services.Customers;

public sealed class CustomerApiService
{
    private readonly FinancialApiClient _apiClient;

    public CustomerApiService(FinancialApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CustomerDto>> GetAllAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var response = await GetAllPagedAsync(
            includeInactive: includeInactive,
            pageNumber: 1,
            pageSize: int.MaxValue,
            cancellationToken: cancellationToken);

        return response.Items.ToList();
    }

    public async Task<PaginatedResponse<CustomerDto>> GetAllPagedAsync(
        bool includeInactive = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var queryParams = new Dictionary<string, string?>
        {
            ["includeInactive"] = includeInactive.ToString().ToLowerInvariant(),
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = pageSize.ToString()
        };

        var url = QueryHelpers.AddQueryString("api/customers", queryParams);

        var result = await client.GetFromJsonAsync<PaginatedResponse<CustomerDto>>(url, cancellationToken);
        return result ?? new PaginatedResponse<CustomerDto>([], 0, pageNumber, pageSize, 0);
    }

    public async Task<int> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.PostAsJsonAsync("api/customers", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Customer creation failed."
                    : errorText);
        }

        var payload = await response.Content.ReadFromJsonAsync<IdResponse>(cancellationToken: cancellationToken);
        return payload?.Id ?? 0;
    }

    public async Task<CustomerDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var result = await client.GetFromJsonAsync<CustomerDto>($"api/customers/{id}", cancellationToken);
        return result;
    }

    public async Task UpdateAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.PutAsJsonAsync($"api/customers/{id}", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Customer update failed."
                    : errorText);
        }
    }

    public async Task DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.PutAsJsonAsync($"api/customers/{id}/deactivate", new { }, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Customer deactivation failed."
                    : errorText);
        }
    }

    public async Task ActivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.PutAsJsonAsync($"api/customers/{id}/activate", new { }, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Customer activation failed."
                    : errorText);
        }
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();

        var response = await client.DeleteAsync($"api/customers/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Customer deletion failed."
                    : errorText);
        }
    }

    private sealed record IdResponse(int Id);
}