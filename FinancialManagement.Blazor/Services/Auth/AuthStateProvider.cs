using FinancialManagement.Blazor.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinancialManagement.Blazor.Services.Auth;

/// <summary>
/// Custom AuthenticationStateProvider for Blazor Server.
/// Persists tokens in ProtectedSessionStorage (cleared when the browser tab closes).
/// Proactively refreshes the access token when it is within 60 seconds of expiry.
/// </summary>
public sealed class AuthStateProvider : AuthenticationStateProvider
{
    private const string StorageKey = "auth_tokens";
    // Refresh if access token expires within this window
    private static readonly TimeSpan RefreshWindow = TimeSpan.FromSeconds(60);

    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly TokenStore _tokenStore;
    private readonly AuthApiService _authApiService;

    public AuthStateProvider(
        ProtectedSessionStorage sessionStorage,
        TokenStore tokenStore,
        AuthApiService authApiService)
    {
        _sessionStorage = sessionStorage;
        _tokenStore = tokenStore;
        _authApiService = authApiService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Restore from session storage if not yet loaded for this circuit
        if (!_tokenStore.IsAuthenticated)
        {
            try
            {
                var result = await _sessionStorage.GetAsync<AuthTokensDto>(StorageKey);
                if (result.Success && result.Value is not null)
                    _tokenStore.SetTokens(result.Value);
            }
            catch
            {
                // Session storage unavailable during pre-render — return anonymous
                return Anonymous();
            }
        }

        if (!_tokenStore.IsAuthenticated)
            return Anonymous();

        // Proactively refresh when the access token is about to expire
        if (_tokenStore.AccessTokenExpiresAt.HasValue &&
            _tokenStore.AccessTokenExpiresAt.Value - DateTime.UtcNow < RefreshWindow)
        {
            await ProactiveRefreshAsync();
        }

        return _tokenStore.IsAuthenticated
            ? BuildAuthState(_tokenStore.AccessToken!)
            : Anonymous();
    }

    /// <summary>Call after a successful login to persist tokens and notify Blazor.</summary>
    public async Task MarkUserAsAuthenticatedAsync(AuthTokensDto tokens)
    {
        _tokenStore.SetTokens(tokens);
        await _sessionStorage.SetAsync(StorageKey, tokens);
        NotifyAuthenticationStateChanged(Task.FromResult(BuildAuthState(tokens.AccessToken)));
    }

    /// <summary>Call on logout to clear tokens and notify Blazor.</summary>
    public async Task MarkUserAsLoggedOutAsync()
    {
        _tokenStore.Clear();
        await _sessionStorage.DeleteAsync(StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
    }

    /// <summary>Exposes the in-memory token store so layout/pages can access the refresh token for logout.</summary>
    public TokenStore GetTokenStore() => _tokenStore;

    // --- Helpers ---

    private async Task ProactiveRefreshAsync()
    {
        if (string.IsNullOrEmpty(_tokenStore.RefreshToken))
            return;

        var newTokens = await _authApiService.RefreshTokenAsync(_tokenStore.RefreshToken);
        if (newTokens is not null)
            await MarkUserAsAuthenticatedAsync(newTokens);
        else
            await MarkUserAsLoggedOutAsync();
    }

    private static AuthenticationState BuildAuthState(string jwt)
    {
        var claims = ParseClaimsFromJwt(jwt);
        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "jwt",
            nameType: JwtRegisteredClaimNames.UniqueName,
            roleType: ClaimTypes.Role);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private static AuthenticationState Anonymous() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(jwt))
            return [];

        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}
