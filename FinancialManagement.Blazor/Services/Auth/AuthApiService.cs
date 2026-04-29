using FinancialManagement.Blazor.Models.Auth;
using System.Net.Http.Json;

namespace FinancialManagement.Blazor.Services.Auth;

public sealed class AuthApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>Sends login credentials to the API and returns JWT tokens on success.</summary>
    public async Task<AuthTokensDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Use a plain (no auth header) client for login
        var client = _httpClientFactory.CreateClient("FinancialApiAnonymous");

        var response = await client.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var tokens = await response.Content.ReadFromJsonAsync<AuthTokensDto>(cancellationToken);
        return tokens ?? throw new InvalidOperationException("Login response was empty.");
    }

    /// <summary>Exchanges a valid refresh token for a new token pair.</summary>
    public async Task<AuthTokensDto?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Use the anonymous client — the access token may already be expired/missing
        var client = _httpClientFactory.CreateClient("FinancialApiAnonymous");

        var body = new { RefreshToken = refreshToken };
        var response = await client.PostAsJsonAsync("api/auth/refresh", body, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthTokensDto>(cancellationToken);
    }

    /// <summary>Revokes the refresh token on the API (logout).</summary>
    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("FinancialApi");

        var request = new { RefreshToken = refreshToken };
        await client.PostAsJsonAsync("api/auth/logout", request, cancellationToken);
    }
}
