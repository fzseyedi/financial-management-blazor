using FinancialManagement.Blazor.Models.Users;
using FinancialManagement.Blazor.Services.Auth;
using System.Net.Http.Json;

namespace FinancialManagement.Blazor.Services.Users;

public sealed class RoleApiService
{
    private readonly FinancialApiClient _apiClient;

    public RoleApiService(FinancialApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var result = await client.GetFromJsonAsync<List<RoleDto>>("api/roles", cancellationToken);
        return result ?? [];
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        return await client.GetFromJsonAsync<RoleDto>($"api/roles/{id}", cancellationToken);
    }

    public async Task<int> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.PostAsJsonAsync("api/roles", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IdResponse>(cancellationToken);
        return result?.Id ?? 0;
    }

    public async Task UpdateAsync(int id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.PutAsJsonAsync($"api/roles/{id}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.DeleteAsync($"api/roles/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private sealed record IdResponse(int Id);
}
