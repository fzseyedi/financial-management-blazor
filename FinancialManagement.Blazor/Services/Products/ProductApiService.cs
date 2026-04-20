using FinancialManagement.Blazor.Models.Common;
using FinancialManagement.Blazor.Models.Products;
using Microsoft.AspNetCore.WebUtilities;

namespace FinancialManagement.Blazor.Services.Products;

public sealed class ProductApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<ProductDto>> GetAllAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var url = includeInactive
            ? "api/products?includeInactive=true"
            : "api/products";

        var result = await client.GetFromJsonAsync<List<ProductDto>>(url, cancellationToken);
        return result ?? [];
    }

    public async Task<PaginatedResponse<ProductDto>> GetAllPagedAsync(
        bool includeInactive = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var queryParams = new Dictionary<string, string?>
        {
            ["includeInactive"] = includeInactive.ToString().ToLowerInvariant(),
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = pageSize.ToString()
        };

        var url = QueryHelpers.AddQueryString("api/products", queryParams);

        var result = await client.GetFromJsonAsync<PaginatedResponse<ProductDto>>(url, cancellationToken);
        return result ?? new PaginatedResponse<ProductDto>([], 0, pageNumber, pageSize, 0);
    }

    /// <summary>
    /// Creates a new product and returns the generated identifier.
    /// </summary>
    public async Task<int> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var response = await client.PostAsJsonAsync("api/products", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Product creation failed."
                    : errorText);
        }

        var payload = await response.Content.ReadFromJsonAsync<IdResponse>(cancellationToken: cancellationToken);
        return payload?.Id ?? 0;
    }

    /// <summary>
    /// Retrieves a single product by its identifier.
    /// </summary>
    public async Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var result = await client.GetFromJsonAsync<ProductDto>($"api/products/{id}", cancellationToken);
        return result;
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    public async Task UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var response = await client.PutAsJsonAsync($"api/products/{id}", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Product update failed."
                    : errorText);
        }
    }

    /// <summary>
    /// Deactivates a product.
    /// </summary>
    public async Task DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var response = await client.PutAsJsonAsync($"api/products/{id}/deactivate", new { }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Product deactivation failed."
                    : errorText);
        }
    }

    /// <summary>
    /// Activates a product.
    /// </summary>
    public async Task ActivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var response = await client.PutAsJsonAsync($"api/products/{id}/activate", new { }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Product activation failed."
                    : errorText);
        }
    }

    /// <summary>
    /// Deletes a product by its identifier.
    /// </summary>
    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var response = await client.DeleteAsync($"api/products/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Product deletion failed."
                    : errorText);
        }
    }

    private sealed record IdResponse(int Id);
}