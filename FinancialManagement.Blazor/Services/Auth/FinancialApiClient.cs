using System.Net.Http.Headers;

namespace FinancialManagement.Blazor.Services.Auth;

/// <summary>
/// Scoped HTTP client wrapper for authenticated API calls.
/// Injects the Bearer token from the circuit-scoped <see cref="TokenStore"/>
/// on every request, avoiding the DelegatingHandler scoping bug where
/// IHttpClientFactory resolves handlers from its own internal scope
/// instead of the Blazor Server circuit scope.
/// </summary>
public sealed class FinancialApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenStore _tokenStore;

    public FinancialApiClient(IHttpClientFactory httpClientFactory, TokenStore tokenStore)
    {
        _httpClient = httpClientFactory.CreateClient("FinancialApi");
        _tokenStore = tokenStore;
    }

    /// <summary>Returns an HttpClient with the current Bearer token attached.</summary>
    public HttpClient GetClient()
    {
        if (!string.IsNullOrEmpty(_tokenStore.AccessToken))
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);
        else
            _httpClient.DefaultRequestHeaders.Authorization = null;

        return _httpClient;
    }
}
