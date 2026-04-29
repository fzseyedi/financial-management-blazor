using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace FinancialManagement.Blazor.Services.Auth;

/// <summary>
/// DelegatingHandler that attaches the Bearer token from <see cref="TokenStore"/>
/// to every outgoing HTTP request made by the named "FinancialApi" client.
/// On a 401 response it attempts a token refresh once and retries the request.
/// If refresh fails, it clears the auth state (forces re-login).
/// </summary>
public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly TokenStore _tokenStore;
    private readonly AuthApiService _authApiService;
    private readonly AuthenticationStateProvider _authStateProvider;

    // Prevents multiple concurrent refreshes from racing
    private static readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthHeaderHandler(
        TokenStore tokenStore,
        AuthApiService authApiService,
        AuthenticationStateProvider authStateProvider)
    {
        _tokenStore = tokenStore;
        _authApiService = authApiService;
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        AttachToken(request);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshed = await TryRefreshAsync(cancellationToken);
            if (refreshed)
            {
                // Clone and retry — the original request body may have already been consumed
                var retry = await CloneRequestAsync(request);
                AttachToken(retry);
                return await base.SendAsync(retry, cancellationToken);
            }
        }

        return response;
    }

    private void AttachToken(HttpRequestMessage request)
    {
        if (!string.IsNullOrEmpty(_tokenStore.AccessToken))
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);
    }

    private async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tokenStore.RefreshToken))
            return false;

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            // Another thread may have already refreshed while we waited
            if (!string.IsNullOrEmpty(_tokenStore.AccessToken) &&
                _tokenStore.AccessTokenExpiresAt > DateTime.UtcNow.AddSeconds(10))
                return true;

            var newTokens = await _authApiService.RefreshTokenAsync(_tokenStore.RefreshToken, cancellationToken);
            if (newTokens is null)
            {
                // Refresh token is expired or invalid — force logout
                if (_authStateProvider is AuthStateProvider asp)
                    await asp.MarkUserAsLoggedOutAsync();
                return false;
            }

            if (_authStateProvider is AuthStateProvider provider)
                await provider.MarkUserAsAuthenticatedAsync(newTokens);

            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    /// <summary>Clones the request so it can be resent (the body stream is not rewindable).</summary>
    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);

        foreach (var header in original.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (original.Content is not null)
        {
            var bytes = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}
