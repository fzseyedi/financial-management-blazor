using FinancialManagement.Blazor.Models.Users;
using FinancialManagement.Blazor.Services.Auth;
using System.Net.Http.Json;

namespace FinancialManagement.Blazor.Services.Users;

public sealed class UserApiService
{
    private readonly FinancialApiClient _apiClient;

    public UserApiService(FinancialApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var result = await client.GetFromJsonAsync<List<UserDto>>("api/users", cancellationToken);
        return result ?? [];
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        return await client.GetFromJsonAsync<UserDto>($"api/users/{id}", cancellationToken);
    }

    public async Task<int> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.PostAsJsonAsync("api/users", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(errorText) ? "User creation failed." : errorText);
        }

        var payload = await response.Content.ReadFromJsonAsync<IdResponse>(cancellationToken: cancellationToken);
        return payload?.Id ?? 0;
    }

    public async Task UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.PutAsJsonAsync($"api/users/{id}", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(errorText) ? "User update failed." : errorText);
        }
    }

    public async Task ChangePasswordAsync(int id, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.PatchAsJsonAsync($"api/users/{id}/password", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(errorText) ? "Password change failed." : errorText);
        }
    }

    public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.PatchAsJsonAsync($"api/users/{id}/activate", new { }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(errorText) ? "User activation failed." : errorText);
        }
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.PatchAsJsonAsync($"api/users/{id}/deactivate", new { }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(errorText) ? "User deactivation failed." : errorText);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = _apiClient.GetClient();
        var response = await client.DeleteAsync($"api/users/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(errorText) ? "User deletion failed." : errorText);
        }
    }

    private sealed record IdResponse(int Id);
}
