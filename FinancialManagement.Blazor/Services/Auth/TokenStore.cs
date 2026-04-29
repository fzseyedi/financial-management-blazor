using FinancialManagement.Blazor.Models.Auth;

namespace FinancialManagement.Blazor.Services.Auth;

/// <summary>
/// Holds auth tokens in memory for the lifetime of a Blazor Server circuit.
/// Scoped per-circuit so each connected user has their own token state.
/// </summary>
public sealed class TokenStore
{
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? AccessTokenExpiresAt { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public void SetTokens(AuthTokensDto tokens)
    {
        AccessToken = tokens.AccessToken;
        RefreshToken = tokens.RefreshToken;
        AccessTokenExpiresAt = tokens.AccessTokenExpiresAt;
    }

    public void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        AccessTokenExpiresAt = null;
    }
}
